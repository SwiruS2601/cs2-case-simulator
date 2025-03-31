/**
 * Global type definitions for window objects added by external scripts
 */

declare interface Window {
    adsbygoogle: Array<{
        push: (params: Record<string, unknown>) => void;
    }>;
}
