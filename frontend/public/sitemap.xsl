<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:html="http://www.w3.org/TR/REC-html40"
                xmlns:sitemap="http://www.sitemaps.org/schemas/sitemap/0.9"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes"/>
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml" lang="en">
      <head>
        <title>XML Sitemap - CS2 Case Simulator</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <style type="text/css">
          body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen, Ubuntu, Cantarell, "Fira Sans", "Droid Sans", "Helvetica Neue", sans-serif;
            color: #333;
            margin: 0;
            padding: 20px;
            line-height: 1.5;
          }
          .container {
            max-width: 1200px;
            margin: 0 auto;
          }
          h1 {
            color: #1a202c;
            font-size: 1.5rem;
            font-weight: 500;
            margin-bottom: 0.5rem;
          }
          p {
            margin-top: 0.5rem;
            color: #4a5568;
          }
          table {
            border: none;
            border-collapse: collapse;
            width: 100%;
            margin-top: 1rem;
          }
          th {
            background-color: #f7fafc;
            border-bottom: 1px solid #edf2f7;
            padding: 0.75rem;
            text-align: left;
            font-size: 0.875rem;
            font-weight: 600;
            color: #4a5568;
          }
          td {
            padding: 0.75rem;
            border-bottom: 1px solid #edf2f7;
            font-size: 0.875rem;
          }
          tr:hover {
            background-color: #f7fafc;
          }
          .url {
            max-width: 70%;
            word-break: break-all;
          }
          .lastmod {
            width: 30%;
          }
          a {
            color: #3182ce;
            text-decoration: none;
          }
          a:hover {
            text-decoration: underline;
          }
          @media (max-width: 640px) {
            table {
              table-layout: fixed;
            }
            .lastmod {
              width: 40%;
            }
            .url {
              max-width: 60%;
            }
          }
        </style>
      </head>
      <body>
        <div class="container">
          <h1>XML Sitemap</h1>
          <p>This XML Sitemap contains <xsl:value-of select="count(sitemap:urlset/sitemap:url)"/> URLs for CS2 Case Simulator.</p>
          <table cellpadding="5" role="presentation">
            <thead>
              <tr>
                <th scope="col">URL</th>
                <th scope="col">Last Modified</th>
              </tr>
            </thead>
            <tbody>
              <xsl:for-each select="sitemap:urlset/sitemap:url">
                <tr>
                  <td class="url">
                    <a href="{sitemap:loc}" target="_blank" rel="noopener">
                      <xsl:value-of select="sitemap:loc"/>
                    </a>
                  </td>
                  <td class="lastmod">
                    <xsl:value-of select="sitemap:lastmod"/>
                  </td>
                </tr>
              </xsl:for-each>
            </tbody>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
