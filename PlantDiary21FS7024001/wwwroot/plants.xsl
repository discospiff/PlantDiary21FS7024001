<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <meta charset="utf-8" />
        <title>My Plants</title>
      </head>
      <body>
        <div>
          <h1>My Favorite Plants!</h1>
        </div>
        <table>
          <tr>
            <th>Latitude</th>
            <th>Longitude</th>
            <th>Planted By</th>
            <th>Description</th>
          </tr>
          <xsl:for-each select ="/plant/specimens/specimen">
            <tr>
              <td>
                <xsl:value-of select="latitude"/>
              </td>
              <td>
                <xsl:value-of select="longitude"/>
              </td>
              <td>
                <xsl:value-of select="planted_by"/>
              </td>
              <td>
                <xsl:value-of select="comments"/>
              </td>              
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>