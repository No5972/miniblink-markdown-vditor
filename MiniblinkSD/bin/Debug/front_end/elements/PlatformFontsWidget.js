/*
 * Copyright (C) 2013 Google Inc. All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Google Inc. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

/**
 * @constructor
 * @extends {WebInspector.ThrottledWidget}
 * @param {!WebInspector.SharedSidebarModel} sharedModel
 */
WebInspector.PlatformFontsWidget = function(sharedModel)
{
    WebInspector.ThrottledWidget.call(this);
    this.element.classList.add("platform-fonts");

    this._sharedModel = sharedModel;
    this._sharedModel.addEventListener(WebInspector.SharedSidebarModel.Events.ComputedStyleChanged, this.update, this);

    this._sectionTitle = createElementWithClass("div", "sidebar-separator");
    this.element.appendChild(this._sectionTitle);
    this._sectionTitle.textContent = WebInspector.UIString("Rendered Fonts");
    this._fontStatsSection = this.element.createChild("div", "stats-section");
}

/**
 * @param {!WebInspector.SharedSidebarModel} sharedModel
 * @return {!WebInspector.ElementsSidebarViewWrapperPane}
 */
WebInspector.PlatformFontsWidget.createSidebarWrapper = function(sharedModel)
{
    var widget = new WebInspector.PlatformFontsWidget(sharedModel);
    return new WebInspector.ElementsSidebarViewWrapperPane(WebInspector.UIString("Fonts"), widget)
}

WebInspector.PlatformFontsWidget.prototype = {
    /**
     * @override
     * @param {!WebInspector.Throttler.FinishCallback} finishedCallback
     * @protected
     */
    doUpdate: function(finishedCallback)
    {
        var cssModel = this._sharedModel.cssModel();
        var node = this._sharedModel.node();
        if (!node || !cssModel) {
            finishedCallback();
            return;
        }
        cssModel.platformFontsPromise(node.id)
            .then(this._refreshUI.bind(this, node))
            .then(finishedCallback)
            .catch(/** @type {function()} */(finishedCallback));
    },

    /**
     * @param {!WebInspector.DOMNode} node
     * @param {?Array.<!CSSAgent.PlatformFontUsage>} platformFonts
     */
    _refreshUI: function(node, platformFonts)
    {
        if (this._sharedModel.node() !== node)
            return;

        this._fontStatsSection.removeChildren();

        var isEmptySection = !platformFonts || !platformFonts.length;
        this._sectionTitle.classList.toggle("hidden", isEmptySection);
        if (isEmptySection)
            return;

        platformFonts.sort(function (a, b) {
            return b.glyphCount - a.glyphCount;
        });
        for (var i = 0; i < platformFonts.length; ++i) {
            var fontStatElement = this._fontStatsSection.createChild("div", "font-stats-item");

            var fontNameElement = fontStatElement.createChild("span", "font-name");
            fontNameElement.textContent = platformFonts[i].familyName;

            var fontDelimeterElement = fontStatElement.createChild("span", "delimeter");
            fontDelimeterElement.textContent = "\u2014";

            var fontUsageElement = fontStatElement.createChild("span", "font-usage");
            var usage = platformFonts[i].glyphCount;
            fontUsageElement.textContent = usage === 1 ? WebInspector.UIString("%d glyph", usage) : WebInspector.UIString("%d glyphs", usage);
        }
    },

    __proto__: WebInspector.ThrottledWidget.prototype
}
