import revealSoundRare from '../assets/audio/case_reveal_rare_01.mp3';
import revealSoundMythical from '../assets/audio/case_reveal_mythical_01.mp3';
import revealSoundLegendary from '../assets/audio/case_reveal_legendary_01.mp3';
import revealSoundAncient from '../assets/audio/case_reveal_ancient_01.mp3';
import unlockSound from '../assets/audio/case_unlock_01.mp3';
import unlockImmidiateSound from '../assets/audio/case_unlock_immediate_01.mp3';
import csgoUiCrateItemScroll from '../assets/audio/csgo_ui_crate_item_scroll.mp3';
import { useOptionsStore } from '~/composables/optionsStore';

const revealSoundMapping = {
  rarity_common_weapon: revealSoundRare,
  rarity_uncommon_weapon: revealSoundRare,
  rarity_rare: revealSoundRare,
  rarity_rare_weapon: revealSoundRare,
  rarity_mythical: revealSoundMythical,
  rarity_mythical_weapon: revealSoundMythical,
  rarity_legendary: revealSoundLegendary,
  rarity_legendary_weapon: revealSoundLegendary,
  rarity_ancient: revealSoundAncient,
  rarity_ancient_weapon: revealSoundAncient,
  exceedingly_rare: revealSoundAncient,
};

const volume = 0.07;

export const audioService = {
  playItemScrollSound,
  playRevealSound,
  playUnlockSound,
  playUnlockImmidiateSound,
};

function playItemScrollSound() {
  try {
    const options = useOptionsStore();
    if (options.soundOn === false) return;
    const audio = new Audio(csgoUiCrateItemScroll);
    audio.volume = volume;
    audio.play();
    audio.onended = () => audio.remove();
  } catch (error) {
    console.error(error);
  }
}

function playRevealSound(rarity: keyof typeof revealSoundMapping) {
  try {
    const options = useOptionsStore();
    if (options.soundOn === false) return;
    const audio = new Audio(revealSoundMapping[rarity]);
    audio.volume = volume;
    audio.play();
    audio.onended = () => audio.remove();
  } catch (error) {
    console.error(error);
  }
}

function playUnlockSound() {
  try {
    const options = useOptionsStore();
    if (options.soundOn === false) return;
    const audio = new Audio(unlockSound);
    audio.volume = volume;
    audio.play();
    audio.onended = () => audio.remove();
  } catch (error) {
    console.error(error);
  }
}

function playUnlockImmidiateSound() {
  try {
    const options = useOptionsStore();
    if (options.soundOn === false) return;
    const audio = new Audio(unlockImmidiateSound);
    audio.volume = volume;
    audio.play();
    audio.onended = () => audio.remove();
  } catch (error) {
    console.error(error);
  }
}
