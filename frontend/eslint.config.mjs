// @ts-check
import withNuxt from './.nuxt/eslint.config.mjs';

export default withNuxt({
    rules: {
        '@typescript-eslint/no-dynamic-delete': 'off',
        'vue/html-self-closing': [
            'error',
            {
                html: {
                    void: 'always',
                    normal: 'never',
                    component: 'never',
                },
            },
        ],
    },
});
