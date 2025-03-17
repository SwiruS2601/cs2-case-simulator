export function useHandleResize(fn: () => void) {
    onMounted(() => {
        fn();
        window.addEventListener('resize', fn);
    });

    onUnmounted(() => {
        window.removeEventListener('resize', fn);
    });
}
