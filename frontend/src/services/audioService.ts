import itemScrollSound from '../assets/audio/csgo_ui_crate_item_scroll.wav';
import revealSoundRare from '../assets/audio/case_reveal_rare_01.wav';
import revealSoundMythical from '../assets/audio/case_reveal_mythical_01.wav';
import revealSoundLegendary from '../assets/audio/case_reveal_legendary_01.wav';
import revealSoundAncient from '../assets/audio/case_reveal_ancient_01.wav';
import unlockSound from '../assets/audio/case_unlock_01.wav';
import unlockImmidiateSound from '../assets/audio/case_unlock_immediate_01.wav';
import { RARITY_MAPPED } from '@/constants';
import { useOptionsStore } from '@/store/optionsStore';

const volume = 0.07;

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
  let audio;
  switch (RARITY_MAPPED[rarity]) {
    case 'rare':
      audio = new Audio(revealSoundRare);
      break;
    case 'mythical':
      audio = new Audio(revealSoundMythical);
      break;
    case 'legendary':
      audio = new Audio(revealSoundLegendary);
      break;
    case 'ancient':
      audio = new Audio(revealSoundAncient);
      break;
    default:
      console.error('Invalid rarity:', rarity);
      return;
  }
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

export const audioService = {
  playItemScrollSound,
  playRevealSound,
  playUnlockSound,
  playUnlockImmidiateSound,
};
