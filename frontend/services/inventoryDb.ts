import { openDB, deleteDB, type IDBPDatabase, type DBSchema } from 'idb';
import { RARITY_ORDER } from '~/constants';

export type InventoryItem = {
    item_id: string;
    name: string;
    rarity_id: string;
    wear_category?: string;
    image?: string;
    price?: number;
    timestamp: number;
    rarity_order?: number;
};

type InventoryDB = DBSchema & {
    items: {
        key: number;
        value: InventoryItem;
        indexes: {
            'by-rarity': string;
            'by-name': string;
            'by-price': number;
            'by-timestamp': number;
            'by-rarity-order': number;
        };
    };
};

let dbPromise: Promise<IDBPDatabase<InventoryDB>> | null = null;

function getDB(name = 'cs2-inventory-db', version = 2) {
    if (!dbPromise) {
        dbPromise = openDB<InventoryDB>(name, version, {
            upgrade(db) {
                if (db.objectStoreNames.contains('items')) {
                    db.deleteObjectStore('items');
                }

                const itemStore = db.createObjectStore('items', { autoIncrement: true });

                itemStore.createIndex('by-rarity', 'rarity_id');
                itemStore.createIndex('by-name', 'name');
                itemStore.createIndex('by-price', 'price');
                itemStore.createIndex('by-timestamp', 'timestamp');
                itemStore.createIndex('by-rarity-order', 'rarity_order');
            },
        });
    }
    return dbPromise;
}

async function resetDatabase() {
    if (dbPromise) {
        (await dbPromise).close();
        dbPromise = null;
    }

    await deleteDB('cs2-inventory-db');

    return getDB();
}

async function addItem(item: InventoryDB['items']['value']) {
    const db = await getDB();
    const rarityOrder = getRarityOrder(item.rarity_id);
    item.rarity_order = rarityOrder;
    return db.add('items', item);
}

async function updateItem(key: number, skin: Partial<InventoryDB['items']['value']>) {
    const db = await getDB();
    const original = await db.get('items', key);
    if (!original) return null;

    return db.put(
        'items',
        {
            ...original,
            ...skin,
        },
        key,
    );
}

async function deleteItem(key: number) {
    const db = await getDB();
    return db.delete('items', key);
}

async function getItem(key: number) {
    const db = await getDB();
    return db.get('items', key);
}

function getRarityOrder(rarityId: string) {
    return RARITY_ORDER[rarityId] || 0;
}

async function getItems({
    sortBy = 'latest',
    page = 1,
    pageSize = 50,
}: {
    sortBy?: 'latest' | 'name' | 'rarity' | 'price';
    page?: number;
    pageSize?: number;
}) {
    const db = await getDB();
    const offset = (page - 1) * pageSize;

    const totalCount = await db.count('items');

    const items: InventoryDB['items']['value'][] = [];

    switch (sortBy) {
        case 'latest': {
            const index = db.transaction('items').store.index('by-timestamp');
            let cursor = await index.openCursor(null, 'prev');

            if (cursor && offset > 0) {
                cursor = await cursor.advance(offset);
            }

            while (cursor && items.length < pageSize) {
                items.push(cursor.value);
                cursor = await cursor.continue();
            }
            break;
        }

        case 'name': {
            const index = db.transaction('items').store.index('by-name');
            let cursor = await index.openCursor();

            if (cursor && offset > 0) {
                cursor = await cursor.advance(offset);
            }

            while (cursor && items.length < pageSize) {
                items.push(cursor.value);
                cursor = await cursor.continue();
            }
            break;
        }

        case 'rarity': {
            const index = db.transaction('items').store.index('by-rarity-order');
            let cursor = await index.openCursor(null, 'prev');

            if (cursor && offset > 0) {
                cursor = await cursor.advance(offset);
            }

            while (cursor && items.length < pageSize) {
                items.push(cursor.value);
                cursor = await cursor.continue();
            }
            break;
        }

        case 'price': {
            const index = db.transaction('items').store.index('by-price');
            let cursor = await index.openCursor(null, 'prev');

            if (cursor && offset > 0) {
                cursor = await cursor.advance(offset);
            }

            while (cursor && items.length < pageSize) {
                items.push(cursor.value);
                cursor = await cursor.continue();
            }
            break;
        }
    }

    return {
        items,
        totalCount,
        page,
        pageSize,
        totalPages: Math.ceil(totalCount / pageSize),
    };
}

async function getItemCountByRarity() {
    const db = await getDB();
    const result: Record<string, number> = {};

    const transaction = db.transaction('items', 'readonly');
    const store = transaction.objectStore('items');
    let cursor = await store.openCursor();

    while (cursor) {
        const rarityId = cursor.value.rarity_id;
        result[rarityId] = (result[rarityId] || 0) + 1;
        cursor = await cursor.continue();
    }

    return result;
}

async function clearItems() {
    const db = await getDB();
    return db.clear('items');
}

async function getItemsCount() {
    const db = await getDB();
    return db.count('items');
}

export const inventoryDb = {
    addItem,
    updateItem,
    deleteItem,
    getItem,
    getItems,
    clearItems,
    getItemsCount,
    getItemCountByRarity,
    resetDatabase,
};
