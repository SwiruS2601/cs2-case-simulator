<script setup lang="ts">
import { useInventoryStore } from '../store/inventoryStore';
import Button from './Button.vue';
const inventory = useInventoryStore();
</script>

<template>
  <header class="sticky top-0 z-[101] backdrop-blur-xs bg-black/50 shadow-2xl border-b border-black/10">
    <nav class="flex items-center justify-between w-full max-w-5xl p-4 mx-auto">
      <router-link to="/">
        <Button>Home</Button>
      </router-link>
      <div>
        Balance:
        <span :class="inventory.balance > 0 ? 'text-green-400' : 'text-red-400'"
          >€ {{ inventory?.balance?.toFixed(2) }}</span
        >
      </div>
      <router-link to="/inventory">
        <Button>Inventory</Button>
      </router-link>
    </nav>
  </header>
</template>
