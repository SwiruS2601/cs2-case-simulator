<script setup lang="ts">
import type { Crate } from '~/types';

defineProps<{
  modelValue: Crate[];
}>();

const emit = defineEmits<{
  'update:modelValue': [value: Crate[]];
}>();

const searchInput = ref('');

let debounceTimeout: ReturnType<typeof setTimeout> | null = null;

watch(searchInput, async (newValue) => {
  if (debounceTimeout) {
    clearTimeout(debounceTimeout);
  }

  if (!newValue || newValue.trim() === '') {
    emit('update:modelValue', []);
    return;
  }

  debounceTimeout = setTimeout(async () => {
    try {
      const results = await $fetch<Crate[]>(
        `${useRuntimeConfig().public.apiUrl}/api/crates/search/${encodeURIComponent(newValue)}`,
      );
      emit('update:modelValue', results || []);
    } catch (error) {
      console.error('Error fetching search results:', error);
      emit('update:modelValue', []);
    }
  }, 300);
});
</script>

<template>
  <input
    type="text"
    placeholder="Search..."
    class="w-full p-2 border border-black/20 bg-black/30 rounded-lg px-4 flex text-white placeholder:text-white/80"
    v-model="searchInput"
  />
</template>
