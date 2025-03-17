import { defineEventHandler, readBody, createError } from 'h3';

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig();

  const origin = event.node.req.headers['origin'] || event.node.req.headers['referer'];

  if (!origin || (!origin.includes('oki.gg') && !origin.includes('localhost'))) {
    console.error('Unauthorized tracking request from', origin);
    throw createError({
      statusCode: 403,
      statusMessage: 'Forbidden',
    });
  }

  try {
    const body = await readBody(event);

    const response = await $fetch(`${config.public.apiUrl}/api/crate-opening/batch`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${config.apiSecret}`,
        'Content-Type': 'application/json',
      },
      body,
    });

    return response;
  } catch (error) {
    console.error('Error proxying tracking request:', error);
    return { success: false, error: 'Failed to process tracking data' };
  }
});
