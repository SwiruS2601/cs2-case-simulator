<script setup lang="ts">
defineProps({
  customClass: {
    type: String,
    default: '',
  },
});
</script>

<template>
  <button
    :class="[
      'px-4 py-1 text-gray-300 border border-gray-400 rounded w-fit hover:border-gray-100 hover:text-gray-100',
      customClass,
    ]"
    v-bind="$attrs"
  >
    <slot></slot>
  </button>
</template>
