<script setup lang="ts">
import { defineWebPage, defineWebSite, useSchemaOrg } from 'nuxt-schema-org/schema';
import Image from '~/components/Image.vue';
import { useCrateOpeningStore } from '~/composables/crateOpeningStore';
import { useInventoryStore } from '~/composables/inventoryStore';
import { useOptionsStore } from '~/composables/optionsStore';
import { REAL_ODDS } from '~/constants';
import { audioService } from '~/services/audioService';
import { crateOpeningService } from '~/services/crateOpeningService';
import type { Crate, Skin } from '~/types';

const router = useRouter();
const name = decodeURIComponent(router.currentRoute.value.params.name as string);
const { data: crate } = await useFetch<Crate>(
  `${useRuntimeConfig().public.apiUrl}/api/crates/name/${encodeURIComponent(name)}`,
);

const title = crate.value?.name ? `${crate.value?.name} - CS2 Case Simulator` : 'CS2 Case Simulator';
const description = crate.value?.name
  ? `Open ${crate.value?.name} for free in this case unboxing simulator.`
  : 'Open Counter-Strike 2 cases for free in this case unboxing simulator.';

const image = crate?.value?.image
  ? `https://images.oki.gg/?url=${encodeURIComponent(crate?.value?.image)}&w=1200`
  : 'https://case.oki.gg/preview.webp';

useSeoMeta({
  viewport: 'width=device-width, initial-scale=1.0',
  title: title,
  ogTitle: title,
  description: description,
  keywords: 'counter strike 2, cs2, case, opening, unboxing, simulator, skins',
  ogDescription: description,
  ogImage: image,
  twitterCard: 'summary_large_image',
  twitterTitle: title,
  twitterDescription: description,
  ogSiteName: title,
  ogUrl: `https://case.oki.gg/crate/${name}`,
  ogType: 'website',
});

const jsonld = {
  '@context': 'https://schema.org',
  '@type': 'Product',
  name: title,
  description: description,
  image: image,
  url: `https://case.oki.gg/crate/${name}`,
  category: crate.value?.type || 'CS2 Case',
  brand: {
    '@type': 'Brand',
    name: 'CS2 Case Simulator',
  },
  offers: {
    '@type': 'Offer',
    price: getCratePrice(crate.value).toFixed(2),
    priceCurrency: 'EUR',
    availability: 'https://schema.org/InStock',
    seller: {
      '@type': 'Organization',
      name: 'CS2 Case Simulator',
    },
  },
  aggregateRating: {
    '@type': 'AggregateRating',
    ratingValue: '4.8',
    reviewCount: '124',
  },
  breadcrumb: {
    '@type': 'BreadcrumbList',
    itemListElement: [
      {
        '@type': 'ListItem',
        position: 1,
        name: 'Home',
        item: 'https://case.oki.gg/',
      },
      {
        '@type': 'ListItem',
        position: 2,
        name: crate.value?.type || 'Cases',
        item: `https://case.oki.gg/${
          crate.value?.type === 'Sticker Capsule'
            ? 'stickers'
            : crate.value?.type === 'Souvenir'
            ? 'souvenirs'
            : crate.value?.type === 'Autograph Capsule'
            ? 'autographs'
            : ''
        }`,
      },
      {
        '@type': 'ListItem',
        position: 3,
        name: crate.value?.name || '',
        item: `https://case.oki.gg/crate/${name}`,
      },
    ],
  },
  hasPart:
    crate.value?.skins?.slice(0, 5).map((skin) => ({
      '@type': 'Product',
      name: skin.name,
      description: `${skin.name} - ${skin.wear_category || 'Skin'} from ${crate.value?.name}`,
      image: skin.image,
      offers: {
        '@type': 'Offer',
        price: getSkinPrice(skin).toFixed(2),
        priceCurrency: 'EUR',
      },
    })) || [],
};

useHead({
  script: [
    {
      hid: 'breadcrumbs-json-ld',
      type: 'application/ld+json',
      textContent: JSON.stringify(jsonld),
    },
  ],
});

const KEY_PRICE = 2.5;

const inventory = useInventoryStore();
const optionsStore = useOptionsStore();
const caseOpeningStore = useCrateOpeningStore();
const crateSliderSkins = ref<Skin[]>([]);
const wonSkinIndex = ref(0);
const showOptions = ref(false);
const wonSkin = ref<Skin | null>(null);
const showWonSkin = ref(false);
const showSlider = ref(false);
const timout = ref(false);
const autoOpen = ref(false);

let autoOpenInterval: ReturnType<typeof setInterval> | null = null;

const guns = computed(() => {
  const skins = crate?.value?.skins;
  if (!skins?.length) return [];
  return skins.filter(gunSkinFilter).sort(sortSkinByRarity);
});

const knivesAndGloves = computed(() => {
  const skins = crate?.value?.skins;
  if (!skins?.length) return [];
  return skins.filter(knivesAndGlovesSkinFilter).sort(sortSkinByName);
});

const handleOpenCase = async () => {
  if (!crate) return;

  handleCloseWonSkinView();

  if (optionsStore.quickOpen) {
    handleQuickOpen();
    return;
  }

  caseOpeningStore.startCaseOpening();

  showOptions.value = false;
  document.body.style.overflow = 'hidden';

  audioService.playUnlockSound();

  await new Promise((resolve) => setTimeout(resolve, 0.1));

  const opnenedCrate = crateOpeningService.openCrate(crate?.value!, REAL_ODDS);

  crateSliderSkins.value = opnenedCrate.sliderSkins;
  wonSkinIndex.value = opnenedCrate.wonSkinIndex;
  wonSkin.value = opnenedCrate.wonSkin;

  showSlider.value = true;
  inventory.incrementOpenCount();
  inventory.setBalance(inventory.balance - (getCratePrice(crate?.value!) + KEY_PRICE));
};

const handleQuickOpen = () => {
  if (!crate) return;

  showSlider.value = false;
  showOptions.value = false;
  document.body.style.overflow = 'hidden';

  audioService.playUnlockImmidiateSound();

  const _wonSkin = crateOpeningService.getWinningSkin(crate?.value!, REAL_ODDS);
  wonSkin.value = _wonSkin;

  if (_wonSkin.rarity_id.includes('ancient') || knivesAndGlovesSkinFilter(_wonSkin)) {
    timout.value = true;
    setTimeout(() => {
      timout.value = false;
    }, 2000);
  }

  handleCaseOpeningFinished(_wonSkin);
  inventory.incrementOpenCount();
  inventory.setBalance(inventory.balance - (getCratePrice(crate?.value!) + KEY_PRICE));
};

const handleCaseOpeningFinished = (skin: Skin) => {
  showWonSkin.value = true;
  caseOpeningStore.endCaseOpening(skin);
  inventory.setBalance(inventory.balance + getSkinPrice(skin));
  inventory.addSkin(skin);
  audioService.playRevealSound(skin.rarity_id);
};

const handleBack = () => {
  handleCloseWonSkinView();
  autoOpen.value = false;
};

const handleCloseWonSkinView = () => {
  document.body.style.overflow = '';
  showWonSkin.value = false;
  showSlider.value = false;
  wonSkin.value = null;
  caseOpeningStore.isOpeningCase = false;
};

const escListener = (event: KeyboardEvent) => {
  if (event.key === 'Escape') handleCloseWonSkinView();
};

const handleClickQuickOpen = () => {
  optionsStore.quickOpen = !optionsStore.quickOpen;
  autoOpen.value = false;
};

watch(autoOpen, (newVal) => {
  if (newVal) {
    autoOpenInterval = setInterval(() => {
      if (!caseOpeningStore.isOpeningCase && !timout.value) {
        handleOpenCase();
      }
    }, 150);
  } else if (autoOpenInterval) {
    clearInterval(autoOpenInterval);
    autoOpenInterval = null;
  }
});

onMounted(() => {
  document.addEventListener('keyup', escListener);
});

onUnmounted(() => {
  if (autoOpenInterval) clearInterval(autoOpenInterval);
  if (wonSkin.value && !caseOpeningStore.wonSkin) {
    handleCaseOpeningFinished(wonSkin.value);
  }
  handleCloseWonSkinView();
  document.removeEventListener('keyup', escListener);
});
</script>

<template>
  <div v-bind="$attrs">
    <Container v-if="!caseOpeningStore.isOpeningCase && !showWonSkin">
      <h1 class="text-xl pb-4">{{ crate?.name }}</h1>
      <div class="flex gap-4 flex-wrap items-center">
        <Backbutton />
        <Button variant="success" @click="handleOpenCase" :disabled="caseOpeningStore.isOpeningCase">
          Unlock Container
        </Button>
        <Image :width="128" :height="96" :src="crate?.image!" :alt="crate?.name" className="h-10 sm:h-11 w-auto" />
      </div>

      <dialog
        v-if="showOptions"
        class="absolute bg-gray-800/95 inset-0 z-50 left-[20%] border border-black/20 top-[74px] flex p-4 rounded-lg shadow-2xl flex-col gap-4"
      >
        <Button @click="optionsStore.toggleFastAnimation">
          {{ optionsStore.fastAnimation ? 'Disable' : 'Enable' }} Fast Animation
        </Button>
        <Button @click="optionsStore.toggleSound"> {{ optionsStore.soundOn ? 'Disable' : 'Enable' }} Sound </Button>
      </dialog>

      <SkinGrid v-if="guns.length" :skins="guns" class="mt-6" />

      <Button v-if="knivesAndGloves.length" @click="optionsStore.toggleShowKnivesAndGloves" class="mt-8">
        {{ optionsStore.showKnivesAndGloves ? 'Hide' : 'Show' }} Knives & Gloves
      </Button>

      <SkinGrid
        class="mt-6"
        v-if="knivesAndGloves.length && optionsStore.showKnivesAndGloves"
        :skins="knivesAndGloves"
      />
    </Container>

    <div
      v-if="(showWonSkin && wonSkin) || caseOpeningStore.isOpeningCase"
      class="max-w-5xl px-4 py-4 mx-auto relative z-100"
    >
      <Button class="flex items-center gap-2" @click="handleBack"><BackIcon /> Back </Button>
    </div>

    <div
      v-if="(caseOpeningStore.isOpeningCase || wonSkin) && showSlider"
      class="fixed inset-0 h-dvh flex items-center justify-center z-40 backdrop-blur-xs"
    >
      <div v-if="crate" class="w-full max-w-5xl">
        <CaseOpeningSlider
          :crate="crate"
          :skins="crateSliderSkins"
          :wonSkinIndex="wonSkinIndex"
          @finished="handleCaseOpeningFinished"
        />
      </div>
    </div>

    <div
      v-if="showWonSkin && wonSkin"
      class="absolute inset-0 h-dvh flex items-center justify-center p-4 z-[70] fade-scale-up backdrop-blur-xs"
    >
      <div class="flex items-center flex-col gap-6 rounded-xl pt-[15dvh] sm:pt-0">
        <img :src="wonSkin?.image" class="select-none sm:max-w-[70%] lg:max-w-[85%] xl:max-w-[100%]" />
        <div class="flex flex-col gap-4 items-center pb-6 pt-2">
          <div
            class="sm:mb-6 relative overflow-hidden bg-black/50 rounded-xl border border-black/10 sm:justify-normal justify-center w-fit"
          >
            <div class="absolute bottom-0 w-full h-2" :style="{ background: getSkinRarityColor(wonSkin) }"></div>
            <div class="flex items-center gap-x-4 gap-y-1.5 flex-col sm:flex-row p-5 pb-6">
              <p class="text-md font-semibold text-white border border-white/30 rounded px-2">
                {{ wonSkin.name }}
              </p>
              <div class="flex gap-1 justify-between items-center">
                <p class="text-sm translate-y-[1px] sm:translate-y-0 text-white/80">{{ wonSkin.wear_category }}</p>
                <p class="text-sm text-white/80">
                  <span class="text-green-400 font-semibold text-lg">€ {{ getSkinPrice(wonSkin).toFixed(2) }} </span>
                </p>
              </div>
            </div>
          </div>
          <div class="flex flex-wrap gap-4 justify-center sm:justify-normal">
            <Button @click="handleBack">Close</Button>
            <Button size="icon" @click="optionsStore.toggleSound">
              <MuteIcon v-if="!optionsStore.soundOn" fill="#fb2c36" class="size-5" />
              <SoundIcon v-if="optionsStore.soundOn" fill="#f0f0f0" class="size-5" />
            </Button>
            <Button
              :style="{ border: autoOpen ? '1px solid #05df72' : '1px solid #0000001a' }"
              size="icon"
              v-if="optionsStore.quickOpen"
              @click="autoOpen = !autoOpen"
            >
              <AutomaticIcon :fill="autoOpen ? '#05df72' : '#f0f0f0'" class="size-6" />
            </Button>
            <Button
              :style="{
                border: optionsStore.quickOpen ? '1px solid #05df72' : '1px solid #0000001a',
                paddingRight: optionsStore.quickOpen ? '0.75rem' : '',
                color: optionsStore.quickOpen ? '#05df72' : '',
              }"
              :disabled="caseOpeningStore.isOpeningCase"
              @click="handleClickQuickOpen"
            >
              Quick Open
              <CheckMarkIcon v-if="optionsStore.quickOpen" class="size-5 ml-2" />
            </Button>
            <Button
              :variant="autoOpen ? 'danger' : 'success'"
              :disabled="caseOpeningStore.isOpeningCase || timout"
              @click="autoOpen ? (autoOpen = false) : handleOpenCase()"
              >{{ autoOpen ? 'Stop' : 'Open Again' }}
            </Button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes fadeScaleUp {
  0% {
    opacity: 0;
    transform: scale(0.4);
  }
  100% {
    opacity: 1;
    transform: scale(1);
  }
}

.fade-scale-up {
  animation: fadeScaleUp 0.18s ease-out forwards;
}

.radial-fade {
  background: radial-gradient(ellipse at center, rgba(0, 0, 0, 0.5), transparent 70%);
}
</style>
