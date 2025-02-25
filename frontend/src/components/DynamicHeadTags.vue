<script setup lang="ts">
import { useRoute } from 'vue-router';
import { watch, computed, onMounted } from 'vue';
import { useCreates } from '../query/crate';

const route = useRoute();

const { data: crates } = useCreates();

const pageType = computed(() => {
  if (route.path === '/') return 'home';
  if (route.path.includes('/crate/')) return 'crate';
  if (route.path.includes('/inventory')) return 'inventory';
  return 'general';
});

const crate = computed(() => crates?.value?.find((c) => c.name === decodeURIComponent(route.params.id as string)));

const schemaData = computed(() => {
  if (!crate.value) return null;

  const base = {
    '@context': 'https://schema.org',
  };

  if (pageType.value === 'crate' && crate.value) {
    return {
      ...base,
      '@type': 'Product',
      name: `${crate.value.name} - CS2 Case Simulator`,
      description: `Open the ${crate.value.name} for free in this Counter-Strike 2 case unboxing simulator`,
      image: crate.value.image,
      offers: {
        '@type': 'Offer',
        price: '0',
        priceCurrency: 'USD',
      },
    };
  }

  return null;
});

const updateMetaTags = () => {
  let title = 'Counter-Strike 2 Case Simulator';
  let description = 'Open Counter-Strike 2 cases for free in this case unboxing simulator.';

  if (pageType.value === 'crate' && crate.value) {
    const crateName = decodeURIComponent(route.params.id as string);
    title = `${crateName} - CS2 Case Simulator`;
    description = `Open the ${crateName} and unbox a skin in this CS2 case simulator.`;
  } else if (pageType.value === 'inventory') {
    title = 'Your Inventory - CS2 Case Simulator';
    description = 'View your unboxed CS2 skins in your personal inventory.';
  }

  document.getElementById('meta-title')!.textContent = title;
  document.getElementById('meta-description')!.setAttribute('content', description);
  document.getElementById('og-meta-title')!.setAttribute('content', title);
  document.getElementById('og-meta-description')!.setAttribute('content', description);
};

watch(
  () => route,
  () => {
    updateMetaTags();
  },
  { immediate: true, deep: true },
);

onMounted(() => {
  updateMetaTags();
  if (schemaData.value && pageType.value !== 'home') {
    const script = document.createElement('script');
    script.setAttribute('type', 'application/ld+json');
    script.textContent = JSON.stringify(schemaData.value);
    script.setAttribute('data-vue-router-controlled', '');
    document.head.appendChild(script);
  }
});
</script>

<template>
  <div></div>
</template>
