<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  variant?: 'primary' | 'secondary' | 'success' | 'warning' | 'danger';
  size?: 'small' | 'medium' | 'large';
}>();

const buttonClasses = computed(() => {
  const base = 'px-4 py-2 flex items-center border rounded w-fit focus:outline-none font-semibold';
  const variants: Record<string, string> = {
    primary: 'bg-slate-600 text-slate-300 border-slate-700 hover:bg-slate-500',
    secondary: 'bg-gray-600 text-white border-gray-600 hover:bg-gray-900',
    success: 'bg-green-400 text-green-900 border-green-400 hover:bg-green-300 hover:text-green-700',
    warning: 'bg-yellow-500 text-white border-yellow-500 hover:bg-yellow-600',
    danger: 'bg-red-600 text-white border-red-600 hover:bg-red-900',
  };
  let sizeClass = props.size === 'small' ? 'text-sm' : props.size === 'large' ? 'text-lg' : 'text-base';
  return [base, variants[props.variant || 'primary'], sizeClass].join(' ');
});
</script>

<template>
  <button :class="[buttonClasses, $attrs.class]" v-bind="$attrs">
    <slot></slot>
  </button>
</template>
