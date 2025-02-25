<script setup lang="ts">
import { useRoute } from 'vue-router';
import { computed, onMounted } from 'vue';

const route = useRoute();
const pageType = computed(() => {
  if (route.path === '/') return 'home';
  if (route.path.includes('/crate/')) return 'crate';
  return 'general';
});

const schemaData = computed(() => {
  const base = {
    '@context': 'https://schema.org',
  };

  if (pageType.value === 'crate' && route.params.id) {
    return {
      ...base,
      '@type': 'Product',
      name: `${route.params.id} Case - CS2 Simulator`,
      description: `Open the ${route.params.id} case for free in this CS2 case simulator`,
      image: `https://case.oki.gg/images/${route.params.id}.webp`,
      offers: {
        '@type': 'Offer',
        price: '0',
        priceCurrency: 'USD',
      },
    };
  }

  return null;
});

onMounted(() => {
  if (schemaData.value && pageType.value !== 'home') {
    const script = document.createElement('script');
    script.setAttribute('type', 'application/ld+json');
    script.textContent = JSON.stringify(schemaData.value);
    script.setAttribute('data-vue-router-controlled', '');
    document.head.appendChild(script);

    return () => {
      document.querySelectorAll('script[data-vue-router-controlled]').forEach((el) => el.remove());
    };
  }
});
</script>

<template>
  <div style="display: none"></div>
</template>
