// import revealSoundRare from '../assets/audio/case_reveal_rare_01.wav';
// import revealSoundMythical from '../assets/audio/case_reveal_mythical_01.wav';
// import revealSoundLegendary from '../assets/audio/case_reveal_legendary_01.wav';
// import revealSoundAncient from '../assets/audio/case_reveal_ancient_01.wav';
// import unlockSound from '../assets/audio/case_unlock_01.wav';
// import unlockImmidiateSound from '../assets/audio/case_unlock_immediate_01.wav';
import { useOptionsStore } from '../store/optionsStore';

const revealSoundMapping = {
  rarity_common_weapon: '/audio/case_reveal_rare_01.wav',
  rarity_uncommon_weapon: '/audio/case_reveal_rare_01.wav',
  rarity_rare: '/audio/case_reveal_rare_01.wav',
  rarity_rare_weapon: '/audio/case_reveal_rare_01.wav',
  rarity_mythical: '/audio/case_reveal_mythical_01.wav',
  rarity_mythical_weapon: '/audio/case_reveal_mythical_01.wav',
  rarity_legendary: '/audio/case_reveal_legendary_01.wav',
  rarity_legendary_weapon: '/audio/case_reveal_legendary_01.wav',
  rarity_ancient: '/audio/case_reveal_ancient_01.wav',
  rarity_ancient_weapon: '/audio/case_reveal_ancient_01.wav',
  exceedingly_rare: '/audio/case_reveal_ancient_01.wav',
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
    const audio = new Audio('/audio/csgo_ui_crate_item_scroll.wav');
    audio.volume = volume;
    audio.play();
    audio.onended = () => audio.remove();
  } catch (error) {
    console.error(error);
  }
}

function playRevealSound(rarity: string) {
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
    const audio = new Audio('/audio/case_unlock_01.wav');
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
    const audio = new Audio('/audio/case_unlock_immediate_01.wav');
    audio.volume = volume;
    audio.play();
    audio.onended = () => audio.remove();
  } catch (error) {
    console.error(error);
  }
}
