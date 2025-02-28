import type { App } from 'vue';

export function extractNonce(): string {
  const nonceElement = document.querySelector('[nonce]');
  return nonceElement?.getAttribute('nonce') || '';
}

export const CspNoncePlugin = {
  install: (app: App) => {
    const nonce = extractNonce();
    app.config.globalProperties.$cspNonce = nonce;

    app.provide('cspNonce', nonce);
  },
};

export function getNonce(): string {
  return extractNonce();
}
