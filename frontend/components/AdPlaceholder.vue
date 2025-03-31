<template>
    <div
        :class="['ad-placeholder', `ad-placeholder--${size}`, { 'ad-placeholder--fixed': isFixed }]"
        :style="customStyle"
    >
        <div class="ad-placeholder__content">
            <div class="ad-placeholder__label">Ad {{ size }}</div>
            <div class="ad-placeholder__dimensions">{{ dimensions }}</div>
            <div v-if="showControls" class="ad-placeholder__controls">
                <button class="ad-placeholder__button" @click="$emit('toggle')" title="Toggle visibility">Hide</button>
                <div class="ad-placeholder__format-selector">
                    <select @change="onChangeSize">
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

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps({
    size: {
        type: String,
        default: 'banner', // banner, square, leaderboard
        validator: (val: string) => ['banner', 'square', 'leaderboard', 'mobile'].includes(val),
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
});

const emit = defineEmits(['toggle', 'change-size']);

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

<style scoped>
.ad-placeholder {
    background-color: rgba(200, 200, 200, 0.2);
    border: 1px dashed rgba(255, 255, 255, 0.3);
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0.5rem 0;
    overflow: hidden;
    border-radius: 4px;
    backdrop-filter: blur(2px);
}

.ad-placeholder--fixed {
    position: fixed;
    left: 0;
    right: 0;
    z-index: 10;
}

.ad-placeholder--banner {
    height: 90px;
    max-width: 728px;
    width: 100%;
    margin-left: auto;
    margin-right: auto;
}

.ad-placeholder--square {
    height: 250px;
    width: 250px;
    margin-left: auto;
    margin-right: auto;
}

.ad-placeholder--leaderboard,
.ad-placeholder--mobile {
    height: 50px;
    max-width: 320px;
    width: 100%;
    margin-left: auto;
    margin-right: auto;
}

.ad-placeholder__content {
    text-align: center;
    color: rgba(255, 255, 255, 0.7);
    font-family: monospace;
}

.ad-placeholder__label {
    font-size: 1rem;
    font-weight: bold;
}

.ad-placeholder__dimensions {
    font-size: 0.8rem;
}

.ad-placeholder__controls {
    display: flex;
    gap: 0.5rem;
    margin-top: 0.25rem;
    justify-content: center;
}

.ad-placeholder__button {
    background: rgba(0, 0, 0, 0.3);
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 3px;
    color: rgba(255, 255, 255, 0.8);
    padding: 0.1rem 0.5rem;
    font-size: 0.7rem;
    cursor: pointer;
}

.ad-placeholder__format-selector select {
    background: rgba(0, 0, 0, 0.3);
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 3px;
    color: rgba(255, 255, 255, 0.8);
    padding: 0.1rem 0.3rem;
    font-size: 0.7rem;
    cursor: pointer;
}

@media (max-width: 768px) {
    .ad-placeholder--banner {
        height: 50px;
        max-width: 320px;
    }

    .ad-placeholder--square {
        height: 200px;
        width: 200px;
    }

    .ad-placeholder {
        background-color: rgba(0, 0, 0, 0.5);
        border: 1px solid rgba(0, 0, 0, 0.15);
        backdrop-filter: blur(1px);
        border-radius: 8px;
        padding: 4px;
    }
}
</style>
