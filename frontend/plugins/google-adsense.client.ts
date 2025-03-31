/**
 * Google AdSense Plugin for Nuxt
 * This is a client-side only plugin to load the Google AdSense script
 */
export default defineNuxtPlugin(() => {
    // Only execute in client side
    if (import.meta.server) return;

    // Create and inject the AdSense script
    const head = document.head;
    const script = document.createElement('script');
    script.async = true;
    script.crossOrigin = 'anonymous';
    script.src = 'https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=ca-pub-2203615625330226';

    // Insert the script into the head
    head.appendChild(script);

    // Add a callback to detect when script is loaded
    const adsenseLoaded = new Promise((resolve, reject) => {
        script.onload = resolve;
        script.onerror = reject;
    });

    // Provide methods for components to use
    return {
        provide: {
            adsense: {
                isLoaded: () => adsenseLoaded,
                refresh: () => {
                    try {
                        // @ts-expect-error - adsbygoogle is added by the external script
                        if (window.adsbygoogle && window.adsbygoogle.push) {
                            // @ts-expect-error - adsbygoogle is added by the external script
                            (window.adsbygoogle = window.adsbygoogle || []).push({});
                        }
                    } catch (error) {
                        console.error('Failed to refresh AdSense ads:', error);
                    }
                },
            },
        },
    };
});
