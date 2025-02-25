import { useQuery } from '@tanstack/vue-query';
import type { Crate } from '../types';
import { config } from '../config';

export function useCreates() {
  return useQuery<Crate[]>({
    queryKey: ['crates'],
    queryFn: async () => fetch(`${config.apiBaseUrl}/api/crates`).then((res) => res.json()),
  });
}

export function useCreate(id: string) {
  return useQuery<Crate>({
    queryKey: ['crate', id],
    queryFn: async () => fetch(`${config.apiBaseUrl}/api/crates/${id}`).then((res) => res.json()),
  });
}

export function useCreateByName(name: string) {
  return useQuery<Crate>({
    queryKey: ['crate', name],
    queryFn: async () => fetch(`${config.apiBaseUrl}/api/crates/name/${name}`).then((res) => res.json()),
  });
}
