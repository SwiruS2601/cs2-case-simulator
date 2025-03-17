export function useCrateOpenCount() {
    const { data: count, refetch } = useQuery<{ totalCount: number }>({
        key: () => ['count'],
        query: ({ signal }) => $fetch(`${useRuntimeConfig().public.apiUrl}/api/crate-opening/count`, { signal }),
    });

    const interval = ref<NodeJS.Timeout | null>(null);

    onMounted(() => {
        interval.value = setInterval(refetch, 2500);
    });

    onUnmounted(() => {
        clearInterval(interval.value!);
    });

    return count;
}
