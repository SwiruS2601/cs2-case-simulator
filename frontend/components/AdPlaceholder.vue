<script setup lang="ts">
import { computed } from 'vue';
import { useCrateOpeningStore } from '~/composables/crateOpeningStore';

const props = defineProps({
    size: {
        type: String,
        default: 'banner', // banner, square, leaderboard
        validator: (val: string) => ['banner', 'square', 'leaderboard', 'mobile', 'skyscraper'].includes(val),
    },
    isFixed: {
        type: Boolean,
        default: false,
    },
    customStyle: {
        type: Object,
        default: () => ({}),
    },
    showControls: {
        type: Boolean,
        default: true,
    },
    hideDuringOpening: {
        type: Boolean,
        default: true,
    },
});

const emit = defineEmits(['toggle', 'change-size']);

const crateOpeningStore = useCrateOpeningStore();

const shouldHideAd = computed(() => {
    if (props.size === 'skyscraper' || !props.hideDuringOpening) return false;
    return crateOpeningStore.isOpeningCase || crateOpeningStore.wonSkin !== null;
});

const dimensions = computed(() => {
    switch (props.size) {
        case 'banner':
            return '728×90';
        case 'square':
            return '250×250';
        case 'leaderboard':
            return '320×50';
        case 'mobile':
            return '320×50';
        case 'skyscraper':
            return '160×600';
        default:
            return '728×90';
    }
});

const onChangeSize = (event: Event) => {
    const target = event.target as HTMLSelectElement;
    if (target && target.value) {
        emit('change-size', target.value);
    }
};
</script>

<template>
    <div
        :class="[
            'flex relative items-center justify-center overflow-hidden sm:rounded',
            'bg-white/20 border border-dashed border-white/30',
            'mx-auto transition-opacity duration-300',
            { 'opacity-0 pointer-events-none': shouldHideAd },
            { 'fixed left-0 right-0': isFixed },
            {
                'h-[90px] sm:max-w-[728px] w-full': size === 'banner',
                'h-[250px] w-[250px]': size === 'square',
                'h-[50px] sm:max-w-[320px] w-full': size === 'leaderboard' || size === 'mobile',
                'h-[600px] w-[160px]': size === 'skyscraper',
            },
            'md:border-black/15 md:rounded-lg',
        ]"
        :style="customStyle"
    >
        <div class="text-center text-white/70 font-mono">
            <div class="text-base font-bold">Ad {{ size }}</div>
            <div class="text-sm">{{ dimensions }}</div>
            <div v-if="showControls" class="flex gap-2 justify-center">
                <button
                    class="bg-black/30 border border-white/20 rounded px-2 py-0.5 text-[0.7rem] text-white/80 cursor-pointer"
                    title="Toggle visibility"
                    @click="$emit('toggle')"
                >
                    Hide
                </button>
                <div class="format-selector">
                    <select
                        class="bg-black/30 border border-white/20 rounded px-1.5 py-0.5 text-[0.7rem] text-white/80 cursor-pointer"
                        @change="onChangeSize"
                    >
                        <option value="mobile" :selected="size === 'mobile'">Mobile (320×50)</option>
                        <option value="square" :selected="size === 'square'">Square (250×250)</option>
                        <option value="leaderboard" :selected="size === 'leaderboard'">Leaderboard (320×50)</option>
                        <option value="banner" :selected="size === 'banner'">Banner (728×90)</option>
                    </select>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped></style>
