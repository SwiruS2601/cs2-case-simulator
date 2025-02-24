import itemScrollSound from '../assets/audio/csgo_ui_crate_item_scroll.wav';
import revealSoundRare from '../assets/audio/case_reveal_rare_01.wav';
import revealSoundMythical from '../assets/audio/case_reveal_mythical_01.wav';
import revealSoundLegendary from '../assets/audio/case_reveal_legendary_01.wav';
import revealSoundAncient from '../assets/audio/case_reveal_ancient_01.wav';
import unlockSound from '../assets/audio/case_unlock_01.wav';
import unlockImmidiateSound from '../assets/audio/case_unlock_immediate_01.wav';
import { useOptionsStore } from '../store/optionsStore';

const revealSoundMapping = {
  rarity_common_weapon: revealSoundRare,
  rarity_uncommon_weapon: revealSoundRare,
  rarity_rare_weapon: revealSoundRare,
  rarity_mythical_weapon: revealSoundMythical,
  rarity_mythical: revealSoundMythical,
  rarity_legendary_weapon: revealSoundLegendary,
  rarity_legendary: revealSoundLegendary,
  rarity_ancient_weapon: revealSoundAncient,
  rarity_ancient: revealSoundAncient,
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
  const options = useOptionsStore();
  if (options.soundOn === false) return;
  const audio = new Audio(itemScrollSound);
  audio.volume = volume;
  audio.play();
  audio.onended = () => audio.remove();
}

function playRevealSound(rarity: string) {
  const options = useOptionsStore();
  if (options.soundOn === false) return;
  const audio = new Audio(revealSoundMapping[rarity]);
  audio.volume = volume;
  audio.play();
  audio.onended = () => audio.remove();
}

function playUnlockSound() {
  const options = useOptionsStore();
  if (options.soundOn === false) return;
  const audio = new Audio(unlockSound);
  audio.volume = volume;
  audio.play();
  audio.onended = () => audio.remove();
}

function playUnlockImmidiateSound() {
  const options = useOptionsStore();
  if (options.soundOn === false) return;
  const audio = new Audio(unlockImmidiateSound);
  audio.volume = volume;
  audio.play();
  audio.onended = () => audio.remove();
}
