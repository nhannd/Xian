<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output indent="yes" omit-xml-declaration="yes" encoding="utf-8" />

  <xsl:param name="oldDocumentName"/>
  <xsl:param name="exclusionsDocumentName" select="'Exclusions.xml'"/>
  <!--Would rather these were booleans (e.g. true()), but haven't figured out how to set booleans on the command line-->
  <xsl:param name="outputDebugComments" select="'True'" />
  <xsl:param name="mergeMode" select="'False'" />

  <xsl:variable name="oldDocument" select="document($oldDocumentName)" />
  <xsl:variable name="exclusionsDocument" select="document($exclusionsDocumentName)" />
  <xsl:variable name="emptyNodeSet" select="/.." />

  <xsl:template match="/">
    <configuration>

      <xsl:for-each select="configuration/*">
        <xsl:variable name="isCustomSettingNode" select="count(/configuration/configSections/section[@name=name(current())]) > 0" />

        <xsl:choose>
          <xsl:when test="name()='configSections'">
            <xsl:call-template name="merge-configSections" />
          </xsl:when>
          <xsl:when test="name()='applicationSettings'">
            <xsl:call-template name="merge-applicationSettings" />
          </xsl:when>
          <xsl:when test="name()='userSettings'">
            <xsl:call-template name="merge-userSettings" />
          </xsl:when>
          <xsl:when test="name()='connectionStrings'">
            <xsl:call-template name="merge-connectionStrings" />
          </xsl:when>
          <xsl:when test="$isCustomSettingNode=true()">
            <xsl:call-template name="merge-customSettingNode" />
          </xsl:when>
          <xsl:otherwise>
            <!-- things like system.serviceModel, configSections, runtime, system.net -->
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>Node is not currently part of upgrade strategy.  Copied verbatim.</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>

      <xsl:if test="$mergeMode='True'">
        <xsl:call-template name="merge-missingCustomSettingNodes" />
      </xsl:if>

    </configuration>
  </xsl:template>

  <xsl:template name ="merge-configSections">

    <!-- We only have to do this in "merge mode" because otherwise, we only merge in what's already in the "new" app.config -->
    <xsl:choose >
      <xsl:when test="$mergeMode='True'">
        <configSections>
          <sectionGroup name="applicationSettings">
            <xsl:for-each select="/configuration/configSections/sectionGroup[@name='applicationSettings']/section">
              <xsl:copy-of select="current()"/>
            </xsl:for-each>

            <xsl:variable name="currentAppSettingsSections" select="/configuration/configSections/sectionGroup[@name='applicationSettings']/section" />
            <xsl:for-each select="$oldDocument/configuration/configSections/sectionGroup[@name='applicationSettings']/section">
              <xsl:variable name="sectionName" select="@name" />
              <xsl:if test="not($currentAppSettingsSections[@name=$sectionName])">
                <xsl:if test="$outputDebugComments='True'">
                  <xsl:comment>"Old" value (merge mode).</xsl:comment>
                </xsl:if>
                <xsl:copy-of select="current()"/>
              </xsl:if>
            </xsl:for-each>
          </sectionGroup>

          <sectionGroup name="userSettings">
            <xsl:for-each select="/configuration/configSections/sectionGroup[@name='userSettings']/section">
              <xsl:copy-of select="current()"/>
            </xsl:for-each>

            <xsl:variable name="currentAppSettingsSections" select="/configuration/configSections/sectionGroup[@name='userSettings']/section" />
            <xsl:for-each select="$oldDocument/configuration/configSections/sectionGroup[@name='userSettings']/section">
              <xsl:variable name="sectionName" select="@name" />
              <xsl:if test="not($currentAppSettingsSections[@name=$sectionName])">
                <xsl:if test="$outputDebugComments='True'">
                  <xsl:comment>"Old" value (merge mode).</xsl:comment>
                </xsl:if>
                <xsl:copy-of select="current()"/>
              </xsl:if>
            </xsl:for-each>
          </sectionGroup>

          <xsl:for-each select="/configuration/configSections/section">
            <xsl:copy-of select="current()"/>
          </xsl:for-each>

          <xsl:variable name="currentCustomSettingsSections" select="/configuration/configSections/section" />
          <xsl:for-each select="$oldDocument/configuration/configSections/section">
            <xsl:variable name="sectionName" select="@name" />
            <xsl:if test="not($currentCustomSettingsSections[@name=$sectionName])">
              <xsl:if test="$outputDebugComments='True'">
                <xsl:comment>"Old" value (merge mode).</xsl:comment>
              </xsl:if>
              <xsl:copy-of select="current()"/>
            </xsl:if>
          </xsl:for-each>

        </configSections>
      </xsl:when>
      <xsl:otherwise>
        <xsl:copy-of select="current()"/>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template name ="merge-connectionStrings">
    <connectionStrings>
      <xsl:for-each select="current()/*">
        <xsl:choose>
          <xsl:when test="name()='add'">
            <xsl:variable name="connectionStringName" select="@name" />
            <xsl:variable name="oldConnectionString" select="$oldDocument/configuration/connectionStrings/add[@name=$connectionStringName]" />
            <xsl:variable name="exclusionNode" select="$exclusionsDocument/configuration/connectionStrings/connectionString[@name=$connectionStringName]" />
            <xsl:choose>
              <xsl:when test="$oldConnectionString and not($exclusionNode)">
                <xsl:if test="$outputDebugComments='True'">
                  <xsl:comment>"Old" value.</xsl:comment>
                </xsl:if>
                <xsl:copy-of select="$oldConnectionString"/>
              </xsl:when>
              <xsl:when test="$oldConnectionString and $exclusionNode">
                <xsl:if test="$outputDebugComments='True'">
                  <xsl:comment>"New" value. "Old" value exists but was excluded.</xsl:comment>
                </xsl:if>
                <xsl:copy-of select="current()"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:if test="$outputDebugComments='True'">
                  <xsl:comment>"New" value. No "Old" value exists.</xsl:comment>
                </xsl:if>
                <xsl:copy-of select="current()"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:when>
          <xsl:otherwise>
            <!-- it's probably the clear node-->
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"New" value.</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>

      <xsl:if test="$mergeMode='True'">
        <xsl:variable name="oldConnectionStrings" select="$oldDocument/configuration/connectionStrings" />
        <xsl:variable name="currentConnectionStrings" select="/configuration/connectionStrings" />
        <xsl:for-each select="$oldConnectionStrings/add">
          <xsl:variable name="connectionStringName" select="@name" />
          <xsl:if test="not($currentConnectionStrings/add[@name=$connectionStringName])">
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"Old" value (merge mode).</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>

    </connectionStrings>
  </xsl:template>

  <xsl:template name ="merge-applicationSettings">
    <applicationSettings>
      <xsl:for-each select="current()/*">

        <xsl:variable name="isApplicationSetting" select="count(/configuration/configSections/sectionGroup[@name='applicationSettings']/section[@name=name(current())]) > 0" />
        <xsl:choose>
          <xsl:when test="$isApplicationSetting=true()">
            <xsl:call-template name="merge-settings-class">
              <xsl:with-param name="currentClassNode" select="current()" />
              <xsl:with-param name="oldClassNode" select="$oldDocument/configuration/applicationSettings/*[name()=name(current())]" />
              <xsl:with-param name="exclusionClassNode" select="$exclusionsDocument/configuration/applicationSettings/*[name()=name(current())]" />
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <!--It's probably a comment-->
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"New" value.</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:otherwise>
        </xsl:choose>

      </xsl:for-each>

      <xsl:if test="$mergeMode='True'">
        <xsl:variable name="oldSettingsClassNodes" select="$oldDocument/configuration/applicationSettings/*" />
        <xsl:variable name="currentSettingsClassNodes" select="/configuration/applicationSettings/*" />
        <xsl:for-each select="$oldSettingsClassNodes">
          <xsl:variable name="settingsClassName" select="name()" />
          <xsl:if test="not($currentSettingsClassNodes[name()=$settingsClassName])">
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"Old" value (merge mode).</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>
    </applicationSettings>

  </xsl:template>

  <xsl:template name="merge-userSettings">
    <userSettings>
      <xsl:for-each select="current()/*">

        <xsl:variable name="isUserSetting" select="count(/configuration/configSections/sectionGroup[@name='userSettings']/section[@name=name(current())]) > 0" />
        <xsl:choose>
          <xsl:when test="$isUserSetting=true()">
            <xsl:call-template name="merge-settings-class">
              <xsl:with-param name="currentClassNode" select="current()" />
              <xsl:with-param name="oldClassNode" select="$oldDocument/configuration/userSettings/*[name()=name(current())]" />
              <xsl:with-param name="exclusionClassNode" select="$exclusionsDocument/configuration/userSettings/*[name()=name(current())]" />
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <!--It's probably a comment-->
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"New" value.</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:otherwise>
        </xsl:choose>

      </xsl:for-each>

      <xsl:if test="$mergeMode='True'">
        <xsl:variable name="oldSettingsClassNodes" select="$oldDocument/configuration/userSettings/*" />
        <xsl:variable name="currentSettingsClassNodes" select="/configuration/userSettings/*" />
        <xsl:for-each select="$oldSettingsClassNodes">
          <xsl:variable name="settingsClassName" select="name()" />
          <xsl:if test="not($currentSettingsClassNodes[name()=$settingsClassName])">
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"Old" value (merge mode).</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>
    </userSettings>

  </xsl:template>
  <xsl:template name="merge-customSettingNode">
    <xsl:variable name="customSettingNode" select="current()" />
    <xsl:variable name="oldCustomSettingNode" select="$oldDocument/configuration/*[name()=name($customSettingNode)]" />
    <xsl:variable name="exclusionsNode" select="$exclusionsDocument/configuration/*[name()=name($customSettingNode)]" />

    <xsl:choose>
      <xsl:when test="$oldCustomSettingNode and $exclusionsNode">
        <xsl:choose>
          <xsl:when test="count($exclusionsNode/*) > 0">
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"Old" value merged in WITH exclusions.</xsl:comment>
            </xsl:if>
            <xsl:copy>
              <xsl:call-template name="merge-attributes">
                <xsl:with-param name="currentElement" select="$customSettingNode"/>
                <xsl:with-param name="oldElement" select="$oldCustomSettingNode"/>
                <xsl:with-param name="exclusions" select="$exclusionsNode/attributes/attribute"/>
              </xsl:call-template>
              <xsl:call-template name="merge-elements">
                <xsl:with-param name="currentElement" select="$customSettingNode"/>
                <xsl:with-param name="oldElement" select="$oldCustomSettingNode"/>
                <xsl:with-param name="exclusions" select="$exclusionsNode/elements/element"/>
              </xsl:call-template>
            </xsl:copy>
          </xsl:when>
          <xsl:otherwise>
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"New" value. "Old" value exists but was excluded.</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="current()"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="$oldCustomSettingNode and not($exclusionsNode)">
        <xsl:if test="$outputDebugComments='True'">
          <xsl:comment>"Old" value merged in with NO exclusions.</xsl:comment>
        </xsl:if>
        <xsl:copy>
          <xsl:call-template name="merge-attributes">
            <xsl:with-param name="currentElement" select="$customSettingNode"/>
            <xsl:with-param name="oldElement" select="$oldCustomSettingNode"/>
            <xsl:with-param name="exclusions" select="$emptyNodeSet"/>
          </xsl:call-template>
          <xsl:call-template name="merge-elements">
            <xsl:with-param name="currentElement" select="$customSettingNode"/>
            <xsl:with-param name="oldElement" select="$oldCustomSettingNode"/>
            <xsl:with-param name="exclusions" select="$emptyNodeSet"/>
          </xsl:call-template>
        </xsl:copy>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="$outputDebugComments='True'">
          <xsl:comment>"New" value.</xsl:comment>
        </xsl:if>
        <xsl:copy-of select="current()"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="merge-attributes">
    <xsl:param name="currentElement" />
    <xsl:param name="oldElement" />
    <xsl:param name="exclusions" />

    <xsl:for-each select="$currentElement/@*">
      <xsl:variable name="oldAttribute" select="$oldElement/@*[name()=name(current())]" />
      <xsl:variable name="exclusion" select="$exclusions[@name=name(current())]" />
      <xsl:choose>
        <xsl:when test="$oldAttribute and not($exclusion)">
          <xsl:copy-of select="$oldAttribute"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:copy-of select="current()"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="merge-elements">
    <xsl:param name="currentElement" />
    <xsl:param name="oldElement" />
    <xsl:param name="exclusions" />

    <xsl:for-each select="$currentElement/*">
      <!--TODO: what about comments-->
      <xsl:variable name="oldChildElement" select="$oldElement/*[name()=name(current())]" />
      <xsl:variable name="exclusion" select="$exclusions[@name=name(current())]" />
      <xsl:choose>
        <xsl:when test="$oldChildElement and not($exclusion)">
          <xsl:if test="$outputDebugComments='True'">
            <xsl:comment>"Old" value.</xsl:comment>
          </xsl:if>
          <xsl:copy-of select="$oldChildElement"/>
        </xsl:when>
        <xsl:when test="$oldChildElement and $exclusion">
          <xsl:if test="$outputDebugComments='True'">
            <xsl:comment>"New" value. "Old" value exists but was excluded.</xsl:comment>
          </xsl:if>
          <xsl:copy-of select="current()"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="$outputDebugComments='True'">
            <xsl:comment>"New" value.</xsl:comment>
          </xsl:if>
          <xsl:copy-of select="current()"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="merge-settings-class">
    <xsl:param name="currentClassNode" />
    <xsl:param name="oldClassNode" />
    <xsl:param name="exclusionClassNode" />

    <xsl:variable name="exclusionSettingNodes" select="$exclusionClassNode/setting" />

    <xsl:choose>
      <xsl:when test="$oldClassNode and $exclusionClassNode">
        <xsl:choose>
          <xsl:when test="count($exclusionSettingNodes) > 0">
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"Old" class merged in WITH exclusions.</xsl:comment>
            </xsl:if>
            <xsl:call-template name="merge-settings-class-nodes">
              <xsl:with-param name="currentClassNode" select="$currentClassNode" />
              <xsl:with-param name="oldClassNode" select="$oldClassNode" />
              <xsl:with-param name="exclusionSettingNodes" select="$exclusionSettingNodes" />
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:if test="$outputDebugComments='True'">
              <xsl:comment>"New" class. "Old" class exists but was excluded.</xsl:comment>
            </xsl:if>
            <xsl:copy-of select="$currentClassNode"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="$oldClassNode and not($exclusionClassNode)">
        <xsl:if test="$outputDebugComments='True'">
          <xsl:comment>"Old" class merged in WITH NO exclusions.</xsl:comment>
        </xsl:if>
        <xsl:call-template name="merge-settings-class-nodes">
          <xsl:with-param name="currentClassNode" select="$currentClassNode" />
          <xsl:with-param name="oldClassNode" select="$oldClassNode" />
          <xsl:with-param name="exclusionSettingNodes" select="$emptyNodeSet" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="$outputDebugComments='True'">
          <xsl:comment>"New" class.</xsl:comment>
        </xsl:if>
        <xsl:copy-of select="$currentClassNode"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="merge-settings-class-nodes">
    <xsl:param name="currentClassNode" />
    <xsl:param name="oldClassNode" />
    <xsl:param name="exclusionSettingNodes" />

    <xsl:variable name="currentSettings" select="$currentClassNode/setting" />
    <xsl:if test="$currentSettings">
      <xsl:copy>
        <xsl:for-each select="$currentSettings">
          <xsl:variable name="settingName" select="@name"/>
          <xsl:variable name="oldSettingNode" select="$oldClassNode/setting[@name=$settingName]"/>
          <xsl:variable name="exclusionSettingNode" select="$exclusionSettingNodes[@name=$settingName]" />
          <xsl:choose>
            <xsl:when test="$oldSettingNode and not($exclusionSettingNode)">
              <xsl:if test="$outputDebugComments='True'">
                <xsl:comment>"Old" value.</xsl:comment>
              </xsl:if>
              <xsl:copy-of select="$oldSettingNode"/>
            </xsl:when>
            <xsl:when test="$oldSettingNode and $exclusionSettingNode">
              <xsl:if test="$outputDebugComments='True'">
                <xsl:comment>"New" value. "Old" value exists but was excluded.</xsl:comment>
              </xsl:if>
              <xsl:copy-of select="current()"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:if test="$outputDebugComments='True'">
                <xsl:comment>"New" value. "Old" value does not exist.</xsl:comment>
              </xsl:if>
              <xsl:copy-of select="current()"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </xsl:copy>
    </xsl:if>
  </xsl:template>

  <xsl:template name="merge-missingCustomSettingNodes">

    <xsl:variable name="oldCustomSettingNodes" select="$oldDocument/configuration/*" />
    <xsl:variable name="currentCustomSettingNodes" select="/configuration/*" />
    <xsl:for-each select="$oldCustomSettingNodes">
      <xsl:variable name="isCustomSettingNode" select="count($oldDocument/configuration/configSections/section[@name=name(current())]) > 0" />
      <xsl:variable name="customSettingNodeName" select="name()" />
      <xsl:if test="not($currentCustomSettingNodes[name()=$customSettingNodeName])">
        <xsl:if test="$outputDebugComments='True'">
          <xsl:comment>"Old" value (merge mode).</xsl:comment>
        </xsl:if>
        <xsl:copy-of select="current()"/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>