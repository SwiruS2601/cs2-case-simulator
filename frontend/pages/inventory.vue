<script setup lang="ts">
import InventoryGrid from '~/components/InventoryGrid.vue';
import { useInventoryStore } from '~/composables/inventoryStore';
import type { InventoryItem } from '~/services/inventoryDb';

const inventory = useInventoryStore();
const selectedSort = ref('latest');
const currentPage = ref(1);
const pageSize = ref(window?.innerWidth < 768 ? 15 : 35);
const items = ref<InventoryItem[]>([]);
const pagination = ref({
    totalCount: 0,
    totalPages: 0,
});
const isLoading = ref(false);

const selectOptions = [
    { value: 'latest', label: 'Latest' },
    { value: 'price', label: 'Price' },
    { value: 'rarity', label: 'Rarity' },
    { value: 'name', label: 'Name' },
];

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
    <Container>
        <div class="flex justify-between items-center flex-wrap gap-4 pb-5">
            <div class="flex gap-4 items-center flex-wrap">
                <Backbutton />
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
                            class="bg-black/50"
                        >
                            {{ option.label }}
                        </option>
                    </select>
                </div>
            </div>

            <Button @click="handleReset" variant="danger">Reset</Button>
        </div>

        <div class="my-4 flex justify-between items-center">
            <p class="text-lg text-white/90">My Items: {{ pagination.totalCount }}</p>

            <div v-if="pagination.totalPages > 1" class="flex gap-2 items-center">
                <Button
                    @click="prevPage"
                    :disabled="currentPage <= 1"
                    :class="{ 'opacity-50 cursor-not-allowed': currentPage <= 1 }"
                >
                    Previous
                </Button>
                <span class="text-white/90"> {{ currentPage }} / {{ pagination.totalPages }} </span>
                <Button
                    @click="nextPage"
                    :disabled="currentPage >= pagination.totalPages"
                    :class="{ 'opacity-50 cursor-not-allowed': currentPage >= pagination.totalPages }"
                >
                    Next
                </Button>
            </div>
        </div>

        <div class="min-h-[70dvh]">
            <div v-if="isLoading"></div>

            <div v-else-if="items.length > 0">
                <InventoryGrid :items="items" inventoryView />
            </div>

            <div v-else class="text-center py-10">
                <p class="text-white/90 text-lg">Your inventory is empty</p>
            </div>
        </div>

        <div v-if="pagination.totalPages > 1" class="mt-4 flex justify-center gap-2 items-center">
            <Button
                @click="prevPage"
                :disabled="currentPage <= 1"
                :class="{ 'opacity-50 cursor-not-allowed': currentPage <= 1 }"
            >
                Previous
            </Button>
            <span class="text-white/90"> {{ currentPage }} / {{ pagination.totalPages }} </span>
            <Button
                @click="nextPage"
                :disabled="currentPage >= pagination.totalPages"
                :class="{ 'opacity-50 cursor-not-allowed': currentPage >= pagination.totalPages }"
            >
                Next
            </Button>
        </div>
    </Container>
</template>
