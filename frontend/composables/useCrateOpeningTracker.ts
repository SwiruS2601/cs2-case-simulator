import type { Crate, Skin } from '~/types';
import { getClientId } from '~/utils/clientId';

type CrateOpeningEntry = {
    crateId: string;
    skinId: string;
    skinName: string;
    crateName: string;
    wearCategory: string | null | undefined;
    rarity: string;
    timestamp: number;
    paintIndex: number | null | undefined;
};

export function useCrateOpeningTracker() {
    const openingQueue = ref<CrateOpeningEntry[]>([]);
    const isSending = ref(false);

    const BATCH_SIZE = 50;
    const BATCH_INTERVAL = 2500;
    const MAX_QUEUE_SIZE = 500;

    const trackOpening = (crate: Crate, skin: Skin) => {
        openingQueue.value.push({
            crateId: crate.id,
            skinId: skin.id,
            skinName: skin.name,
            crateName: crate.name,
            wearCategory: skin.wear_category,
            rarity: skin.rarity_id,
            timestamp: Date.now(),
            paintIndex: skin.paint_index ? parseInt(skin.paint_index, 10) || null : null,
        });

        if (openingQueue.value.length >= MAX_QUEUE_SIZE) {
            sendBatch();
        }
    };

    const sendBatch = async () => {
        if (isSending.value || openingQueue.value.length === 0) return;

        isSending.value = true;

        const batchSize = Math.min(BATCH_SIZE, openingQueue.value.length);
        const batch = openingQueue.value.slice(0, batchSize);

        try {
            await $fetch('/api/track-crate-opening', {
                method: 'POST',
                body: {
                    openings: batch,
                    clientId: getClientId(),
                },
            });

            openingQueue.value = openingQueue.value.slice(batchSize);
        } catch (err) {
            console.error('Failed to send crate opening batch:', err);
        } finally {
            isSending.value = false;
        }
    };

    let batchInterval: ReturnType<typeof setInterval> | null = null;

    onMounted(() => {
        batchInterval = setInterval(sendBatch, BATCH_INTERVAL);
    });

    onUnmounted(() => {
        if (batchInterval) clearInterval(batchInterval);
        if (openingQueue.value.length > 0) {
            sendBatch();
        }
    });

    if (import.meta.client) {
        window.addEventListener('beforeunload', () => {
            if (openingQueue.value.length > 0) {
                const batch = openingQueue.value;
                navigator.sendBeacon(
                    '/api/track-crate-opening',
                    JSON.stringify({
                        openings: batch,
                        clientId: getClientId(),
                    }),
                );
            }
        });
    }

    return {
        trackOpening,
        queueSize: computed(() => openingQueue.value.length),
        isSending,
        flushQueue: sendBatch,
    };
}
