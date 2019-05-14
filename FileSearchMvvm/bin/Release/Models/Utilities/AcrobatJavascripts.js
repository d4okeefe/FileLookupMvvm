/*  This file is copied from FileSearchMvvm to Acrobat's Javascript folder as:
    C:\Users\daniel\AppData\Roaming\Adobe\Acrobat\Privileged\10.0\JavaScripts\local_scripts.js
    The user acct file will only be updated if there has been a change to the original.
*/
function convertToPdfA() {
    var oProfile = Preflight.getProfileByName('Convert to PDF/A-1b (sRGB)');
    if (oProfile !== undefined) {
        var myPreflightResult = this.preflight(oProfile, false);
        return this.path;
    }
    console.println("\nPreflight profile not found.");
    return -1;
}
function getLetterSizeCoverCoordinates(cover_length) {
    if (cover_length === 48) { return [-117.5, 35, 494.5, 827]; }
    if (cover_length === 49) { return [-117.5, 30, 494.5, 822]; }
    if (cover_length === 50) { return [-117.5, 25, 494.5, 817]; }
    if (cover_length === 51) { return [-117.5, 20, 494.5, 812]; }
    else { return null; }
}
function getBookletSizeCoverCoordinates(cover_length) {
    if (cover_length === 48) { return [-32, 105.5, 409, 771.5]; }
    if (cover_length === 49) { return [-32, 98.5, 409, 764.5]; }
    if (cover_length === 50) { return [-32, 91.5, 409, 757.5]; }
    if (cover_length === 51) { return [-32, 84.5, 409, 750.5]; }
    else { return null; }
}
function getLetterSizeBriefCoordinates() { return [-117.5, 94.5, 494.5, 886.5]; }
function getBookletSizeBriefCoordinates() { return [-32, 157.5, 409, 823.5]; }
function centerPdfOnLetterPageNoCover() {
    var rectBrief = getLetterSizeBriefCoordinates();
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: rectBrief
    });
    var media_box = this.getPageBox('Media');
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: media_box
    });
    return this.path;
}
function centerPdfOnLetterPageWithCover48() {
    var rectCover = getLetterSizeCoverCoordinates(48);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getLetterSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnLetterPageWithCover49() {
    var rectCover = getLetterSizeCoverCoordinates(49);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getLetterSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnLetterPageWithCover50() {
    var rectCover = getLetterSizeCoverCoordinates(50);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getLetterSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnLetterPageWithCover51() {
    var rectCover = getLetterSizeCoverCoordinates(51);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getLetterSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnBriefPageWithNoCover() {
    var rectBrief = getBookletSizeBriefCoordinates();
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: rectBrief
    });
    var media_box = this.getPageBox('Media');
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: media_box
    });
    return this.path;
}
function centerPdfOnBriefPageWithCover48() {
    var rectCover = getBookletSizeCoverCoordinates(48);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getBookletSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnBriefPageWithCover49() {
    var rectCover = getBookletSizeCoverCoordinates(49);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getBookletSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnBriefPageWithCover50() {
    var rectCover = getBookletSizeCoverCoordinates(50);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getBookletSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
function centerPdfOnBriefPageWithCover51() {
    var rectCover = getBookletSizeCoverCoordinates(51);
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: 0,
        rBox: rectCover
    });
    var media_box_cover = this.getPageBox('Media', 0);
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: 0,
        rBox: media_box_cover
    });
    if (this.numPages > 1) {
        var rectBrief = getBookletSizeBriefCoordinates();
        this.setPageBoxes({
            cBox: 'Media',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: rectBrief
        });
        var media_box_brief = this.getPageBox('Media', 1);
        this.setPageBoxes({
            cBox: 'Crop',
            nStart: 1,
            nEnd: this.numPages - 1,
            rBox: media_box_brief
        });
    }
    return this.path;
}
