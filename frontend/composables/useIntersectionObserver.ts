import { onBeforeUnmount, ref } from 'vue';

interface IntersectionState {
    [key: string]: boolean;
}

let observer: IntersectionObserver | null = null;
const observedElements = new Map<Element, { id: string; callback: (isIntersecting: boolean) => void }>();
const intersectionState = ref<IntersectionState>({});

let nextId = 0;
const generateId = () => `intersection-${nextId++}`;

export function useIntersectionObserver() {
    const initObserver = () => {
        if (typeof window === 'undefined' || !('IntersectionObserver' in window) || observer) return;

        observer = new IntersectionObserver(
            (entries) => {
                entries.forEach((entry) => {
                    const element = entry.target;
                    const data = observedElements.get(element);

                    if (data) {
                        intersectionState.value[data.id] = entry.isIntersecting;
                        data.callback(entry.isIntersecting);
                    }
                });
            },
            {
                rootMargin: '50px',
                threshold: 0.01,
            },
        );
    };

    const observe = (element: Element | null, callback: (isIntersecting: boolean) => void) => {
        if (!element) return { id: '', unobserve: () => {} };

        initObserver();

        if (!observer) {
            callback(true);
            return { id: '', unobserve: () => {} };
        }

        const id = generateId();

        observedElements.set(element, { id, callback });

        observer.observe(element);

        intersectionState.value[id] = false;

        return {
            id,
            unobserve: () => {
                if (observer && element) {
                    observer.unobserve(element);
                    observedElements.delete(element);
                    delete intersectionState.value[id];
                }
            },
        };
    };

    onBeforeUnmount(() => {});

    return {
        observe,
        intersectionState,
    };
}
