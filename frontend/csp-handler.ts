import type { Plugin } from 'vite';

export function generateNonce(): string {
  return Buffer.from(Math.random().toString()).toString('base64').substring(0, 16);
}

export function createCspNoncePlugin(): Plugin {
  return {
    name: 'csp-nonce-plugin',
    transformIndexHtml(html: string): string {
      const nonce = generateNonce();
      return html.replace(/nonce="<\?= \$csp_nonce \?>"/g, `nonce="${nonce}"`);
    },
  };
}
