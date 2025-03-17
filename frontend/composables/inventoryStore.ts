import { defineStore } from 'pinia';
import { inventoryDb } from '~/services/inventoryDb';
import type { Skin } from '~/types';

type ItemFetchOptions = {
    sortBy?: 'latest' | 'name' | 'rarity' | 'price';
    page?: number;
    pageSize?: number;
};

export const useInventoryStore = defineStore(
    'inventory-store-v5',
    () => {
        const balance = ref(0);
        const openCount = ref(0);
        const itemCount = ref(0);
        const isLoading = ref(false);

        const setBalance = (newBalance: number) => (balance.value = newBalance);
        const incrementOpenCount = () => openCount.value++;

        const addItem = async (item: Skin) => {
            await inventoryDb.addItem({
                item_id: item.id,
                name: item.name,
                rarity_id: item.rarity_id,
                wear_category: item.wear_category,
                image: item.image,
                price: getItemPrice(item),
                timestamp: Date.now(),
            });
            itemCount.value++;
        };

        const fetchItems = async (options: ItemFetchOptions = { sortBy: 'latest', page: 1, pageSize: 50 }) => {
            isLoading.value = true;
            try {
                return await inventoryDb.getItems(options);
            } finally {
                isLoading.value = false;
            }
        };

        const fetchItemsCount = async () => {
            if (!itemCount.value) {
                itemCount.value = await inventoryDb.getItemsCount();
            }
            return itemCount.value;
        };

        const clearInventory = async () => {
            await inventoryDb.clearItems();
            itemCount.value = 0;
            openCount.value = 0;
            balance.value = 0;
        };

        onMounted(async () => {
            itemCount.value = await inventoryDb.getItemsCount();
        });

        return {
            balance,
            setBalance,
            openCount,
            incrementOpenCount,
            itemCount,
            addItem,
            fetchItems,
            fetchItemsCount,
            clearInventory,
            isLoading,
        };
    },
    {
        persist: {
            pick: ['balance', 'openCount'],
        },
    },
);
