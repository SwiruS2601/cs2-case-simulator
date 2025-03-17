export default {
    env: {
        browser: true,
        es2021: true,
    },
    extends: ['eslint:recommended', 'plugin:react/recommended'],
    parserOptions: {
        sourceType: 'module',
    },
    plugins: ['vue'],
    rules: {},
};
