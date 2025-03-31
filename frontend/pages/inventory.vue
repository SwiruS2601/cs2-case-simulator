<script setup lang="ts">
import InventoryGrid from '~/components/InventoryGrid.vue';
import GoogleAd from '~/components/GoogleAd.vue';
import { useInventoryStore } from '~/composables/inventoryStore';
import { COLOR_ORDER, RARITY_COLORS } from '~/constants';
import { inventoryDb, type InventoryItem } from '~/services/inventoryDb';
import { ref } from 'vue';

const inventory = useInventoryStore();
const selectedSort = ref('latest');
const currentPage = ref(1);
const pageSize = ref(window?.innerWidth < 768 ? 51 : 63);
const items = ref<InventoryItem[]>([]);
const rarityStats = ref<{ color: string; percent: string; count: number }[]>([]);
const isLoading = ref(false);
const pagination = ref({
    totalCount: 0,
    totalPages: 0,
});

const selectOptions = [
    { value: 'latest', label: 'Latest' },
    { value: 'price', label: 'Price' },
    { value: 'rarity', label: 'Rarity' },
    { value: 'name', label: 'Name' },
];

async function loadRarityStats() {
    const rarityCounts = await inventoryDb.getItemCountByRarity();
    const totalItems = await inventoryDb.getItemsCount();

    if (totalItems === 0) {
        rarityStats.value = [];
        return;
    }

    const colorCounts: Record<string, { count: number; color: string }> = {};

    Object.entries(rarityCounts).forEach(([rarityId, count]) => {
        const colorHex = RARITY_COLORS[rarityId] || '#ffffff';

        if (!colorCounts[colorHex]) {
            colorCounts[colorHex] = { count: 0, color: colorHex };
        }

        colorCounts[colorHex].count += count;
    });

    const stats = Object.entries(colorCounts)
        .map(([colorHex, { count }]) => {
            const percent = ((count / totalItems) * 100).toFixed(2);
            return { color: colorHex, percent, count };
        })
        .sort((a, b) => {
            const indexA = COLOR_ORDER.indexOf(a.color);
            const indexB = COLOR_ORDER.indexOf(b.color);

            if (indexA !== -1 && indexB !== -1) {
                return indexA - indexB;
            }

            if (indexA !== -1) return -1;
            if (indexB !== -1) return 1;

            return 0;
        });

    rarityStats.value = stats;
}

async function loadItems() {
    isLoading.value = true;

    try {
        const sortBy = ['latest', 'name', 'rarity', 'price'].includes(selectedSort.value)
            ? selectedSort.value
            : 'latest';

        const result = await inventory.fetchItems({
            sortBy: sortBy as 'latest' | 'name' | 'rarity' | 'price',
            page: currentPage.value,
            pageSize: pageSize.value,
        });

        items.value = result.items;
        pagination.value = {
            totalCount: result.totalCount,
            totalPages: result.totalPages,
        };

        await loadRarityStats();
    } catch (error) {
        console.error('Failed to load inventory:', error);
    } finally {
        isLoading.value = false;
    }
}

const onChange = async (event: Event) => {
    selectedSort.value = (event.target as HTMLSelectElement).value;
    currentPage.value = 1;
    await loadItems();
};

const nextPage = async () => {
    if (currentPage.value < pagination.value.totalPages) {
        currentPage.value++;
        await loadItems();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
};

const prevPage = async () => {
    if (currentPage.value > 1) {
        currentPage.value--;
        await loadItems();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
};

const handleReset = async () => {
    if (confirm('Are you sure you want to reset your inventory?')) {
        isLoading.value = true;
        try {
            await inventory.clearInventory();
            currentPage.value = 1;
            await loadItems();
        } catch (error) {
            console.error('Failed to reset inventory:', error);
        } finally {
            isLoading.value = false;
        }
    }
};

onMounted(() => {
    loadItems();
});
</script>

<template>
    <div>
        <Container>
            <!-- Top inventory ad -->
            <GoogleAd class="block md:hidden" size="mobile" adSlot="inventory-top-mobile"></GoogleAd>
            <div class="px-4 pt-4">
                <div class="flex justify-between items-center gap-4 pb-3">
                    <div class="flex gap-4 items-center flex-wrap">
                        <BackButton></BackButton>
                        <p
                            v-if="pagination.totalCount"
                            class="text-xl text-white/85 absolute left-1/2 transform -translate-x-1/2 sm:relative sm:translate-x-0 sm:left-0"
                        >
                            {{ pagination.totalCount }} Items
                        </p>
                    </div>
                    <ClientOnly>
                        <Button v-if="items.length || inventory.balance" variant="danger" @click="handleReset"
                            >Reset</Button
                        >
                    </ClientOnly>
                </div>

                <div
                    v-if="items.length && rarityStats.length"
                    class="mb-4 flex justify-between items-center flex-col sm:flex-row sm:gap-4"
                >
                    <div
                        class="flex gap-x-2 items-center pb-2 bg-black/20 border border-black/15 rounded-lg py-1 px-2 mr-auto mt-auto flex-wrap"
                    >
                        <div
                            v-for="stat in rarityStats"
                            :key="stat.color"
                            class="flex flex-col items-center drop-shadow-2xl gap-x-1 rounded-xs"
                        >
                            <div>
                                <span class="text-sm font-bold text-white/90"> {{ stat.count }} </span>
                                <span class="text-white/50 px-[3px]">-</span>
                                <span class="text-sm text-white/75 font-semibold"> {{ stat.percent }}% </span>
                            </div>
                            <div class="h-1 w-full rounded-xs" :style="{ background: stat.color }"></div>
                        </div>
                    </div>
                    <div
                        class="flex flex-wrap sm:flex-nowrap gap-4 items-center sm:mt-auto sm:mb-0 mt-4 mb-2 justify-between sm:justify-normal w-full sm:w-auto"
                    >
                        <div class="flex flex-col">
                            <label for="inventory-sort" class="sr-only">Sort inventory by</label>
                            <select
                                id="inventory-sort"
                                class="rounded-lg py-2 w-fit border pr-4 h-[42px] border-black/10 bg-black/20 focus:outline-none px-4 font-semibold cursor-pointer hover:bg-black/10 select:outline-none"
                                @change="onChange"
                            >
                                <option
                                    v-for="option in selectOptions"
                                    :key="option.value"
                                    :value="option.value"
                                    :selected="option.value === selectedSort"
                                    class="bg-black/70"
                                >
                                    {{ option.label }}
                                </option>
                            </select>
                        </div>
                        <div v-if="pagination.totalPages > 1" class="flex gap-2 items-center">
                            <Button
                                :disabled="currentPage <= 1"
                                :class="{ 'opacity-50 cursor-not-allowed': currentPage <= 1 }"
                                @click="prevPage"
                            >
                                Previous
                            </Button>
                            <div class="text-white/90 sm:text-nowrap">
                                {{ currentPage }} / {{ pagination.totalPages }}
                            </div>
                            <Button
                                :disabled="currentPage >= pagination.totalPages"
                                :class="{ 'opacity-50 cursor-not-allowed': currentPage >= pagination.totalPages }"
                                @click="nextPage"
                            >
                                Next
                            </Button>
                        </div>
                    </div>
                </div>

                <div class="min-h-[70dvh]">
                    <div v-if="isLoading"></div>

                    <div v-else-if="items.length > 0">
                        <InventoryGrid :items="items" inventory-view></InventoryGrid>
                    </div>

                    <div v-else class="text-center py-10">
                        <p class="text-white/90 text-lg">Your inventory is empty</p>
                    </div>
                </div>

                <div v-if="pagination.totalPages > 1" class="mt-4 flex justify-center gap-2 items-center">
                    <Button
                        :disabled="currentPage <= 1"
                        :class="{ 'opacity-50 cursor-not-allowed': currentPage <= 1 }"
                        @click="prevPage"
                    >
                        Previous
                    </Button>
                    <span class="text-white/90"> {{ currentPage }} / {{ pagination.totalPages }} </span>
                    <Button
                        :disabled="currentPage >= pagination.totalPages"
                        :class="{ 'opacity-50 cursor-not-allowed': currentPage >= pagination.totalPages }"
                        @click="nextPage"
                    >
                        Next
                    </Button>
                </div>
            </div>
        </Container>
        <GoogleAd size="mobile" adSlot="inventory-bottom-mobile"></GoogleAd>
    </div>
</template>
