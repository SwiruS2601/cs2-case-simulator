import type { Crate } from '~/types';

export function useCrate(name: string) {
  return useQuery<Crate>({
    key: ['crate', name],
    query: ({ signal }) =>
      $fetch(`${useRuntimeConfig().public.apiUrl}/api/crates/name/${encodeURIComponent(name)}`, { signal }),
  });
}
