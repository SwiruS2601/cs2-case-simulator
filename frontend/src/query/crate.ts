import { useQuery } from '@tanstack/vue-query';
import type { Crate } from '../types';

export function useCreates() {
  return useQuery<Crate[]>({
    queryKey: ['crates'],
    queryFn: async () => fetch('http://localhost:5015/api/case').then((res) => res.json()),
  });
}

export function useCreate(id: string) {
  return useQuery<Crate>({
    queryKey: ['crate', id],
    queryFn: async () => fetch(`http://localhost:5015/api/case/${id}`).then((res) => res.json()),
  });
}
