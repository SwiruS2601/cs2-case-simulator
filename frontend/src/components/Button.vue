<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  variant?: 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'cta';
  size?: 'icon' | 'small' | 'medium' | 'large';
}>();

const buttonClasses = computed(() => {
  const base = 'flex items-center border rounded w-fit focus:outline-none font-semibold cursor-pointer';
  const variants = {
    primary: 'bg-slate-600 text-slate-300 border-slate-600 hover:bg-slate-500',
    secondary: 'bg-gray-600 text-white border-gray-600 hover:bg-gray-900',
    success: 'bg-green-400 text-green-900 border-green-400 hover:bg-green-300 hover:text-green-700',
    warning: 'bg-yellow-500 text-white border-yellow-500 hover:bg-yellow-600',
    danger: 'bg-red-500/80 text-white border-red-500 hover:bg-red-500',
    cta: 'bg-yellow-300 text-yellow-950 border-yellow-300 hover:bg-yellow-300/80',
  };
  let sizeClass =
    props.size === 'icon'
      ? 'rounded-full flex items-center justify-center max-w-10 max-h-10 min-w-10 min-h-10'
      : props.size === 'small'
      ? 'text-sm px-4 py-2 '
      : props.size === 'large'
      ? 'text-lg px-4 py-2 '
      : 'text-base px-4 py-2 ';
  return [base, variants[props.variant || 'primary'], sizeClass].join(' ');
});
</script>

<template>
  <button :class="[buttonClasses, $attrs.class]" v-bind="$attrs">
    <slot></slot>
  </button>
</template>
