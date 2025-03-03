import { serverConfig } from '~/config.server';

export default defineEventHandler(async (event) => {
  const input = getRouterParam(event, 'input');
  const crates = await $fetch(`${serverConfig.apiUrl}/api/crates/search/${input}`);
  return crates;
});
