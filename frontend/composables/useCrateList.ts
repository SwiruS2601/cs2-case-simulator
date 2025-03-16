import { useQuery } from '@pinia/colada';
import type { Crate } from '~/types';

type CrateType = 'cases' | 'stickers' | 'souvenirs' | 'autographs';

export function useCrateList(type: CrateType) {
  const { data } = useQuery<Crate[]>({
    key: () => ['crate', type],
    query: ({ signal }) => $fetch(`${useRuntimeConfig().public.apiUrl}/api/crates/${type}`, { signal }),
  });

  const searchData = ref<Crate[] | null>(null);

  const crates = computed(() => {
    if (searchData?.value) return searchData.value;
    return data.value?.length ? data.value : [];
  });

  return {
    data,
    searchData,
    crates,
  };
}
