import { serverConfig } from '~/config.server';

export default defineEventHandler(async () => {
  const crates = await $fetch(`${serverConfig.apiUrl}/api/crates`);
  return crates;
});
