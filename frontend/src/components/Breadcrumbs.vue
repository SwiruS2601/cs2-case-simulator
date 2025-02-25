<script setup lang="ts">
import { computed, watch, onMounted, onBeforeUnmount } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const breadcrumbs = computed(() => {
  const pathParts = route.path.split('/').filter(Boolean);
  const result = [{ name: 'Home', path: '/' }];

  if (pathParts.length === 0) {
    return result;
  }

  let cumulativePath = '';
  pathParts.forEach((part, i) => {
    cumulativePath += `/${part}`;

    if (part === 'crate' && i < pathParts.length - 1) {
      result.push({ name: 'Cases', path: '/' });
    } else if (part === 'inventory') {
      result.push({ name: 'Inventory', path: cumulativePath });
    } else if (i === pathParts.length - 1 && route.params.id) {
      result.push({ name: `${route.params.id} Case`, path: cumulativePath });
    }
  });

  return result;
});

const structuredBreadcrumbs = computed(() => {
  const items = breadcrumbs.value.map((crumb, index) => ({
    '@type': 'ListItem',
    position: index + 1,
    name: crumb.name,
    item: `https://case.oki.gg${crumb.path}`,
  }));

  return {
    '@context': 'https://schema.org',
    '@type': 'BreadcrumbList',
    itemListElement: items,
  };
});

let scriptElement: HTMLScriptElement | null = null;

const updateJsonLd = () => {
  if (scriptElement) {
    document.head.removeChild(scriptElement);
    scriptElement = null;
  }

  if (breadcrumbs.value.length <= 1) return;

  scriptElement = document.createElement('script');
  scriptElement.setAttribute('type', 'application/ld+json');
  scriptElement.textContent = JSON.stringify(structuredBreadcrumbs.value);
  document.head.appendChild(scriptElement);
};

watch(() => structuredBreadcrumbs.value, updateJsonLd, { immediate: true });

onMounted(updateJsonLd);
onBeforeUnmount(() => {
  if (scriptElement) {
    document.head.removeChild(scriptElement);
  }
});
</script>

<template>
  <nav
    v-if="breadcrumbs.length > 1"
    aria-label="Breadcrumb"
    class="w-full max-w-5xl px-4 mx-auto mt-4 mb-6 breadcrumbs-container"
  >
    <ol class="flex items-center text-sm text-white/80">
      <li v-for="(crumb, index) in breadcrumbs" :key="index" class="flex items-center">
        <router-link v-if="index < breadcrumbs.length - 1" :to="crumb.path" class="hover:text-white transition-colors">
          {{ crumb.name }}
        </router-link>
        <span v-else>{{ crumb.name }}</span>
        <span v-if="index < breadcrumbs.length - 1" class="mx-2">/</span>
      </li>
    </ol>
  </nav>
  <div v-else class="hidden"></div>
</template>

<style scoped>
.breadcrumbs-container {
  min-height: auto;
  padding: 0.5rem 0;
}

.breadcrumbs-container:empty,
.hidden {
  padding: 0;
  margin: 0;
  height: 0;
}
</style>
