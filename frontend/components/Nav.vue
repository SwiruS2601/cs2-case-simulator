<script setup lang="ts">
import { useInventoryStore } from '~/composables/inventoryStore';
import Button from './Button.vue';

defineOptions({ name: 'NavigationBar' });
const inventory = useInventoryStore();
</script>

<template>
    <header class="sticky top-0 z-[101] backdrop-blur-xs bg-black/50 shadow-2xl border-b border-black/10">
        <nav class="flex items-center justify-between w-full max-w-5xl p-4 mx-auto">
            <router-link to="/">
                <Button>Home</Button>
            </router-link>
            <div class="flex gap-2 items-center">
                <span class="sm:block hidden">Balance:</span>
                <ClientOnly>
                    <span
                        :class="
                            inventory.balance === 0
                                ? 'text-gray-200'
                                : inventory.balance > 0
                                ? 'text-green-400'
                                : 'text-red-400'
                        "
                    >
                        {{ formatEuro(inventory?.balance) }}
                    </span>
                    <template #fallback>
                        <span class="text-gray-400">0.00 €</span>
                    </template>
                </ClientOnly>
            </div>
            <router-link to="/inventory">
                <Button>Inventory</Button>
            </router-link>
        </nav>
    </header>
</template>
