<script setup lang="ts">
import { ref } from 'vue';
import { useQuery } from '@pinia/colada';
import Container from '~/components/Container.vue';
import Button from '~/components/Button.vue';

type ClientStatistic = {
    clientIp: string;
    rowCount: number;
};

type CountryStatistic = {
    country: string;
    count: number;
    entries: ClientStatistic[];
};

const config = useRuntimeConfig();

const fetchStatistics = async (): Promise<CountryStatistic[]> => {
    const response = await fetch(`${config.public.apiUrl}/api/statistics`);
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    return await response.json();
};

const {
    data: statistics,
    isPending: pending,
    error,
} = useQuery({
    key: ['statistics'],
    query: fetchStatistics,
});

const expandedCountries = ref<Set<string>>(new Set());

function toggleCountry(country: string) {
    if (expandedCountries.value.has(country)) {
        expandedCountries.value.delete(country);
    } else {
        expandedCountries.value.add(country);
    }
}

function isExpanded(country: string): boolean {
    return expandedCountries.value.has(country);
}
</script>

<template>
    <Container>
        <div class="px-4 py-6 sm:px-6 lg:px-8">
            <h1 class="text-xl font-semibold pb-4 text-white">Case Opening Statistics</h1>

            <div v-if="pending" class="text-center py-10 text-gray-400">Loading statistics...</div>
            <div v-else-if="error" class="text-center py-10 text-red-500">
                Failed to load statistics: {{ error.message }}
            </div>
            <div v-else-if="statistics && statistics.length > 0" class="mt-4 border-t border-gray-700">
                <template v-for="stat in statistics" :key="stat.country">
                    <div
                        class="grid grid-cols-[minmax(0,_1fr)_auto_auto] items-center gap-x-6 py-4 pl-4 pr-4 sm:pl-6 sm:pr-6 border-b border-gray-700 hover:bg-gray-700/50 transition-colors duration-150"
                    >
                        <div class="whitespace-nowrap text-sm font-medium text-white">
                            {{ stat.country }}
                        </div>
                        <div class="whitespace-nowrap text-sm text-gray-300">
                            {{ stat.count }}
                        </div>
                        <div class="justify-self-end">
                            <Button
                                variant="text"
                                size="small"
                                class="!border-none !px-2 !py-1"
                                @click="toggleCountry(stat.country)"
                            >
                                {{ isExpanded(stat.country) ? 'Hide' : 'Show' }} Details
                                <span class="sr-only">, {{ stat.country }}</span>
                            </Button>
                        </div>
                    </div>
                    <div v-if="isExpanded(stat.country)" class="bg-gray-850 border-b border-gray-700">
                        <div class="py-4 pl-4 pr-4 sm:pl-6 sm:pr-6">
                            <ul class="divide-y divide-gray-700 text-sm text-gray-400">
                                <li
                                    v-for="entry in stat.entries"
                                    :key="entry.clientIp"
                                    class="py-2 flex justify-between"
                                >
                                    <span
                                        class="font-mono text-gray-300 mr-4 overflow-hidden text-ellipsis whitespace-nowrap"
                                        >{{ entry.clientIp }}</span
                                    >
                                    <span>{{ entry.rowCount }}</span>
                                </li>
                            </ul>
                        </div>
                    </div>
                </template>
            </div>
            <div v-else class="text-center py-10 text-gray-500">No statistics available yet.</div>
        </div>
    </Container>
</template>

<style scoped>
.bg-gray-850 {
    background-color: #111827;
}

:deep(button[variant='text']) {
}
</style>
