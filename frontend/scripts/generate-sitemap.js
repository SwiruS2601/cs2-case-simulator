import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

// Get the equivalent of __dirname in ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

async function generateSitemap() {
  try {
    // Fetch crate data from API
    const response = await fetch('https://caseapi.oki.gg/api/crates');
    const crates = await response.json();

    const today = new Date().toISOString().split('T')[0];

    let sitemapContent = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        xmlns:image="http://www.google.com/schemas/sitemap-image/1.1"
        xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9
        http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd
        http://www.google.com/schemas/sitemap-image/1.1
        http://www.google.com/schemas/sitemap-image/1.1/sitemap-image.xsd">

  <!-- Homepage -->
  <url>
    <loc>https://case.oki.gg/</loc>
    <lastmod>${today}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>1.0</priority>
    <image:image>
      <image:loc>https://case.oki.gg/preview.webp</image:loc>
      <image:title>Counter-Strike 2 Case Simulator</image:title>
    </image:image>
  </url>
  
  <!-- Inventory page -->
  <url>
    <loc>https://case.oki.gg/inventory</loc>
    <lastmod>${today}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.8</priority>
  </url>`;

    crates.forEach((crate) => {
      sitemapContent += `
  
  <!-- ${crate.name} -->
  <url>
    <loc>https://case.oki.gg/crate/${crate.id}</loc>
    <lastmod>${today}</lastmod>
    <changefreq>monthly</changefreq>
    <priority>0.7</priority>
    <image:image>
      <image:loc>https://case.oki.gg${crate.image}</image:loc>
      <image:title>${crate.name}</image:title>
    </image:image>
  </url>`;
    });

    sitemapContent += `
</urlset>`;

    fs.writeFileSync(path.resolve(__dirname, '../public/sitemap.xml'), sitemapContent);
    console.log('Sitemap generated successfully!');
  } catch (error) {
    console.error('Error generating sitemap:', error);
  }
}

generateSitemap();
