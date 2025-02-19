import itemScrollSound from '../assets/audio/csgo_ui_crate_item_scroll.wav';
import revealSoundRare from '../assets/audio/case_reveal_rare_01.wav';
import revealSoundMythical from '../assets/audio/case_reveal_mythical_01.wav';
import revealSoundLegendary from '../assets/audio/case_reveal_legendary_01.wav';
import revealSoundAncient from '../assets/audio/case_reveal_ancient_01.wav';
import unlockSound from '../assets/audio/case_unlock_01.wav';
import unlockImmidiateSound from '../assets/audio/case_unlock_immediate_01.wav';
import { useOptionsStore } from '../store/optionsStore';
import { RARITY_MAPPED } from '../constants';

const revealSoundMapping = {
  rare: revealSoundRare,
  mythical: revealSoundMythical,
  legendary: revealSoundLegendary,
  ancient: revealSoundAncient,
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

function playRevealSound(rarity: keyof typeof RARITY_MAPPED) {
  const options = useOptionsStore();
  if (options.soundOn === false) return;
  const audio = new Audio(revealSoundMapping[RARITY_MAPPED[rarity]]);
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
