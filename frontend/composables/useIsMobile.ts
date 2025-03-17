export function useIsMobile() {
    const isMobile = ref(false);

    const eventHandler = () => {
        isMobile.value = window.innerWidth < 768;
    };

    onMounted(() => {
        eventHandler();
        window.addEventListener('resize', eventHandler);
    });

    onUnmounted(() => {
        window.removeEventListener('resize', eventHandler);
    });

    return isMobile;
}
