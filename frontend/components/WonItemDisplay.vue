<script setup lang="ts">
import { useOptionsStore } from '~/composables/optionsStore';
import type { Crate, Skin } from '~/types';
import { getItemRarityColor } from '~/utils/color';
import { getItemPrice } from '~/utils/currency';

defineProps<{
    crate: Crate;
    item: Skin;
    autoOpen: boolean;
    timeout: boolean;
}>();

const emit = defineEmits<{
    close: [];
    openAgain: [];
    toggleAutoOpen: [];
    quickOpenToggle: [];
}>();

const optionsStore = useOptionsStore();

const handleBack = () => {
    emit('close');
};

const handleOpenAgain = () => {
    emit('openAgain');
};

const toggleAutoOpen = () => {
    emit('toggleAutoOpen');
};

const toggleQuickOpen = () => {
    emit('quickOpenToggle');
};
</script>

<template>
    <div class="absolute inset-0 h-dvh flex items-center justify-center p-4 z-[70] fade-scale-up backdrop-blur-xs">
        <div class="flex items-center flex-col gap-4 rounded-xl">
            <img :src="item?.image" class="select-none max-w-[380px] lg:max-w-[500px] w-full" />
            <div class="flex flex-col gap-4 items-center pb-6 pt-2">
                <div
                    class="sm:mb-6 relative overflow-hidden bg-black/50 rounded-xl border border-black/10 sm:justify-normal justify-center w-fit"
                >
                    <div class="absolute bottom-0 w-full h-2" :style="{ background: getItemRarityColor(item) }"></div>
                    <div class="flex items-center gap-x-4 gap-y-1.5 flex-col sm:flex-row p-5 pb-6">
                        <p class="text-md font-semibold text-white border border-white/30 rounded px-2">
                            {{ item.name }}
                        </p>
                        <div class="flex gap-1 justify-between items-center">
                            <p class="text-sm translate-y-[1px] sm:translate-y-0 text-white/80">
                                {{ item.wear_category === 'Default' ? '' : item.wear_category }}
                            </p>
                            <p class="text-sm text-white/80">
                                <span class="text-green-400 font-semibold text-lg">
                                    {{ formatEuro(getItemPrice(item)) }}
                                </span>
                            </p>
                        </div>
                    </div>
                </div>
                <div class="flex flex-wrap gap-4 justify-center sm:justify-normal">
                    <Button @click="handleBack">Close</Button>
                    <Button size="icon" @click="optionsStore.toggleSound">
                        <MuteIcon v-if="!optionsStore.soundOn" fill="#fb2c36" class="size-5"></MuteIcon>
                        <SoundIcon v-if="optionsStore.soundOn" fill="#f0f0f0" class="size-5"></SoundIcon>
                    </Button>
                    <Button
                        v-if="optionsStore.quickOpen"
                        :style="{ border: autoOpen ? '1px solid #05df72' : '1px solid #0000001a' }"
                        size="icon"
                        @click="toggleAutoOpen"
                    >
                        <AutomaticIcon :fill="autoOpen ? '#05df72' : '#f0f0f0'" class="size-6"></AutomaticIcon>
                    </Button>
                    <Button
                        :style="{
                            border: optionsStore.quickOpen ? '1px solid #05df72' : '1px solid #0000001a',
                            paddingRight: optionsStore.quickOpen ? '0.75rem' : '',
                            color: optionsStore.quickOpen ? '#05df72' : '',
                        }"
                        @click="toggleQuickOpen"
                    >
                        Quick Open
                        <CheckMarkIcon v-if="optionsStore.quickOpen" class="size-5 ml-2"></CheckMarkIcon>
                    </Button>
                    <Button
                        :variant="autoOpen ? 'danger' : 'success'"
                        :disabled="timeout"
                        @click="autoOpen ? toggleAutoOpen() : handleOpenAgain()"
                    >
                        {{ autoOpen ? 'Stop' : 'Open Again' }}
                    </Button>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
@keyframes fadeScaleUp {
    0% {
        opacity: 0;
        transform: scale(0.4);
    }
    100% {
        opacity: 1;
        transform: scale(1);
    }
}

.fade-scale-up {
    animation: fadeScaleUp 0.18s ease-out forwards;
}
</style>
