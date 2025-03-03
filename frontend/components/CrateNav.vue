<script setup lang="ts">
type CrateType = 'Cases' | 'Souvenirs' | 'Stickers' | 'Autographs';
defineProps<{
  activeLink: CrateType;
}>();
</script>

<template>
  <nav class="flex gap-2 flex-wrap">
    <NuxtLink to="/" prefetch-on="interaction">
      <Button :variant="activeLink === 'Cases' ? 'cta' : 'primary'" size="pill">Cases</Button>
    </NuxtLink>
    <NuxtLink to="/souvenirs" prefetch-on="interaction">
      <Button :variant="activeLink === 'Souvenirs' ? 'cta' : 'primary'" size="pill"> Souvenirs </Button>
    </NuxtLink>
    <NuxtLink to="/stickers" prefetch-on="interaction">
      <Button :variant="activeLink === 'Stickers' ? 'cta' : 'primary'" size="pill"> Stickers </Button>
    </NuxtLink>
    <NuxtLink to="/autographs" prefetch-on="interaction">
      <Button :variant="activeLink === 'Autographs' ? 'cta' : 'primary'" size="pill"> Autographs </Button>
    </NuxtLink>
  </nav>
</template>
