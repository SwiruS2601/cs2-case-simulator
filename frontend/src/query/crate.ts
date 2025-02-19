import { useQuery } from '@tanstack/vue-query';
import type { Crate } from '../types';
import { config } from '../config';

export function useCreates() {
  return useQuery<Crate[]>({
    queryKey: ['crates'],
    queryFn: async () => fetch(`${config.apiBaseUrl}/api/case`).then((res) => res.json()),
  });
}

export function useCreate(id: string) {
  return useQuery<Crate>({
    queryKey: ['crate', id],
    queryFn: async () => fetch(`${config.apiBaseUrl}/api/case/${id}`).then((res) => res.json()),
  });
}
