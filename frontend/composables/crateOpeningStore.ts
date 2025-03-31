import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { Skin } from '~/types';

export const useCrateOpeningStore = defineStore('case-opening', () => {
    const isOpeningCase = ref(false);
    const wonSkin = ref<Skin | null>(null);

    const setWonSkin = (skin: Skin | null) => {
        wonSkin.value = skin;
    };

    const startCaseOpening = () => {
        isOpeningCase.value = true;
        wonSkin.value = null;
    };

    const endCaseOpening = (skin: Skin | null) => {
        wonSkin.value = skin;
        isOpeningCase.value = false;
    };

    return {
        isOpeningCase,
        wonSkin,
        startCaseOpening,
        endCaseOpening,
        setWonSkin,
    };
});
