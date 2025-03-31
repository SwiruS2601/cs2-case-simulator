<script setup lang="ts">
import { computed } from 'vue';

defineOptions({ name: 'BaseButton' });

const props = defineProps<{
    variant?: 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'cta' | 'text';
    size?: 'icon' | 'pill' | 'small' | 'medium' | 'large';
}>();

const base = 'flex items-center border rounded w-fit focus:outline-none font-semibold cursor-pointer';

const sizes = {
    icon: 'rounded-full flex items-center justify-center max-w-10 max-h-10 min-w-10 min-h-10',
    pill: 'text-sm rounded-full px-2.5 py-1',
    small: 'text-sm px-4 py-2',
    medium: 'text-base px-4 py-2 rounded-lg',
    large: 'text-lg px-4 py-2',
};

const variants = {
    primary: 'text-white/90 border-black/10 hover:bg-black/20 bg-black/30',
    text: 'text-white border-white hover:bg-black/20',
    secondary: 'bg-gray-600 text-white border-gray-600 hover:bg-gray-900',
    success: 'bg-green-400 text-green-900 border-green-400 hover:bg-green-500 hover:text-green-900',
    warning: 'bg-yellow-500 text-white border-yellow-500 hover:bg-yellow-600',
    danger: 'bg-red-500/80 text-white border-red-500 hover:bg-red-500',
    cta: 'bg-yellow-300 text-yellow-950 border-yellow-900/30',
};

const buttonClasses = computed(() => {
    const sizeClass = sizes[props.size || 'medium'];
    return [base, variants[props.variant || 'primary'], sizeClass].join(' ');
});
</script>

<template>
    <button :class="[buttonClasses, $attrs.class]" v-bind="$attrs">
        <slot></slot>
    </button>
</template>
