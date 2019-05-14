/*  This file is copied from FileSearchMvvm to Acrobat's Javascript folder as:
    C:\Users\daniel\AppData\Roaming\Adobe\Acrobat\Privileged\10.0\JavaScripts\local_scripts.js
    The user acct file will only be updated if there has been a change to the original.

    Import webpage for understanding pdf layout:
    https://www.pdfscripting.com/public/PDF-Page-Coordinates.cfm
*/
function extractPagesFromDocument(start_page, end_page, new_doc_name) {
    if (start_page === undefined) {
        start_page = 0;
    }
    if (end_page === undefined) {
        end_page = start_page;
    }

    this.extractPages(start_page, end_page, new_doc_name);
}
function cropTypesetBriefPages(doc, start_page, end_page) {
    if (start_page === undefined) {
        start_page = 0;
    }
    if (end_page === undefined) {
        end_page = start_page;
    }

    var orig_media_br = doc.getPageBox('Media', start_page);

    var p0 = orig_media_br[0];
    var p1 = orig_media_br[1];
    var p2 = orig_media_br[2] - 235.8;
    var p3 = orig_media_br[3] + 189;

    var new_rect_br = [p0, p1, p2, p3];

    doc.setPageBoxes('Media', start_page, end_page, new_rect_br);
    var m_box_br = doc.getPageBox('Media', start_page);
    doc.setPageBoxes('Crop', start_page, end_page, m_box_br);
}
function centerTypeset50picaCoverWithBriefOnBooklet() {
    // crop cover
    var orig_media_cv = this.getPageBox('Media', 0);
    // get coords for crop
    var p0 = orig_media_cv[0] + 21;
    var p1 = orig_media_cv[1] - 52;
    var p2 = orig_media_cv[2] - 256;
    var p3 = orig_media_cv[3] + 106;

    var new_rect_cv = [p0, p1, p2, p3];

    this.setPageBoxes('Media', 0, 0, new_rect_cv);
    var m_box_cv = this.getPageBox('Media', 0);
    this.setPageBoxes('Crop', 0, 0, m_box_cv);
    // set cover on 6.125 x 9.25
    centerTypsetDocOnBookletPages(this, 0, 0);

    // crop brief 
    if (this.numPages > 1) {
        // crop brief
        cropTypesetBriefPages(this, 1, this.numPages - 1);
        // set brief on 6.125 x 9.25
        centerTypsetDocOnBookletPages(this, 1, this.numPages - 1);
    }

    // weird: have to reset all crop boxes
    for (var i = 0; i < this.numPages; i++) {
        var m_box_i = this.getPageBox('Media', i);
        this.setPageBoxes('Crop', i, i, m_box_i);
    }
}
function centerTypeset51picaCoverWithBriefOnBooklet() {
    // crop cover: can assume typeset, offset to left

    // crop cover
    var orig_media_cv = this.getPageBox('Media', 0);

    var p0 = orig_media_cv[0] + 21;
    var p1 = orig_media_cv[1] - 52;
    var p2 = orig_media_cv[2] - 256;
    var p3 = orig_media_cv[3] + 96;

    var new_rect_cv = [p0, p1, p2, p3];

    this.setPageBoxes('Media', 0, 0, new_rect_cv);
    var m_box_cv = this.getPageBox('Media', 0);
    this.setPageBoxes('Crop', 0, 0, m_box_cv);

    centerTypsetDocOnBookletPages(this, 0, 0);

    // crop brief 
    if (this.numPages > 1) {
        cropTypesetBriefPages(this, 1, this.numPages - 1);
        centerTypsetDocOnBookletPages(this, 1, this.numPages - 1);
    }

    // weird: have to reset all crop boxes
    for (var i = 0; i < this.numPages; i++) {
        var m_box_i = this.getPageBox('Media', i);
        this.setPageBoxes('Crop', i, i, m_box_i);
    }
}
function centerTypsetDocOnBookletPages(doc, start_page, end_page) {
    if (start_page === undefined) {
        start_page = 0;
    }
    if (end_page === undefined) {
        end_page = start_page;
    }
    var orig_rect = doc.getPageBox('Media', start_page);
    var orig_width = orig_rect[2] - orig_rect[0];
    var orig_height = orig_rect[1] - orig_rect[3];

    var booklet_width = 441;
    var booklet_height = 666;

    var amt_subtract_from_width = (orig_width - booklet_width) / 2;
    var amt_subtract_from_height = (orig_height - booklet_height) / 2;

    var p0 = orig_rect[0] + amt_subtract_from_width;
    var p1 = orig_rect[1] - amt_subtract_from_height;
    var p2 = orig_rect[2] - amt_subtract_from_width;
    var p3 = orig_rect[3] + amt_subtract_from_height;

    var new_rect = [p0, p1, p2, p3];

    doc.setPageBoxes({
        cBox: 'Media',
        nStart: start_page,
        nEnd: end_page,
        rBox: new_rect
    });
    var media_box = doc.getPageBox('Media');
    doc.setPageBoxes({
        cBox: 'Crop',
        nStart: start_page,
        nEnd: end_page,
        rBox: media_box
    });
}
function centerCenteredDocOnBookletPages() {
    var orig_rect = this.getPageBox('Media');
    var orig_width = orig_rect[2] - orig_rect[0];
    var orig_height = orig_rect[1] - orig_rect[3];

    var booklet_width = 441;
    var booklet_height = 666;

    var amt_subtract_from_width = (orig_width - booklet_width) / 2;
    var amt_subtract_from_height = (orig_height - booklet_height) / 2;

    var p0 = orig_rect[0] + amt_subtract_from_width;
    var p1 = orig_rect[1] - amt_subtract_from_height;
    var p2 = orig_rect[2] - amt_subtract_from_width;
    var p3 = orig_rect[3] + amt_subtract_from_height;

    var new_rect = [p0, p1, p2, p3];

    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: new_rect
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
function centerCenteredDocOnLetterPages() {
    var orig_rect = this.getPageBox('Media');
    var orig_width = orig_rect[2] - orig_rect[0];
    var orig_height = orig_rect[1] - orig_rect[3];

    var letter_width = 612;
    var letter_height = 792;

    var amt_add_to_width = (letter_width - orig_width) / 2;
    var amt_add_to_height = (letter_height - orig_height) / 2;

    var p0 = orig_rect[0] - amt_add_to_width;
    var p1 = orig_rect[1] + amt_add_to_height;
    var p2 = orig_rect[2] + amt_add_to_width;
    var p3 = orig_rect[3] - amt_add_to_height;

    var new_rect = [p0, p1, p2, p3];

    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: new_rect
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
function arePagesLargerThanBooklet() {
    if (!isFilePageSizeConsistent(this)) {
        return false;
    }
    var booklet_width = 441;
    var booklet_height = 666;
    var doc_rect = getCurrentPageCropBox(this, 0);
    var doc_width = doc_rect[2] - doc_rect[0];
    var doc_height = doc_rect[1] - doc_rect[3];
    return booklet_width < doc_width && booklet_height < doc_height;
}
function arePagesSmallerThanLetter() {
    if (!isFilePageSizeConsistent(this)) {
        return false;
    }
    var let_width = 612;
    var let_height = 792;
    var doc_rect = getCurrentPageCropBox(this, 0);
    var doc_width = doc_rect[2] - doc_rect[0];
    var doc_height = doc_rect[1] - doc_rect[3];
    return let_width > doc_width && let_height > doc_height;
}
function isFilePageSizeConsistent(doc) {
    var isSame = true;
    if (doc.numPages === 1) {
        return isSame;
    }
    else {
        var mediaBx0 = getCurrentPageMediaBox(doc, 0);
        var cropBx0 = getCurrentPageCropBox(doc, 0);
        for (var i = 1; i < doc.numPages - 1; i++) {
            var mediaBxi = getCurrentPageMediaBox(doc, i);
            var cropBxi = getCurrentPageCropBox(doc, i);
            try {
                if (mediaBx0[0] !== mediaBxi[0] || mediaBx0[1] !== mediaBxi[1]
                    || mediaBx0[2] !== mediaBxi[2] || mediaBx0[3] !== mediaBxi[3]) {
                    isSame = false;
                    break;
                }
                if (cropBx0[0] !== cropBxi[0] || cropBx0[1] !== cropBxi[1]
                    || cropBx0[2] !== cropBxi[2] || cropBx0[3] !== cropBxi[3]) {
                    isSame = false;
                    break;
                }
            }
            catch (err) {
                isSame = false;
                break;
            }
        }
        return isSame;
    }
}
function getCurrentPageCropBox(doc, pg_num) {
    var cropBx0 = doc.getPageBox('Crop', pg_num);
    return cropBx0;
}
function getCurrentPageMediaBox(doc, pg_num) {
    var mediaBx0 = doc.getPageBox('Media', pg_num);
    return mediaBx0;
}
function convertToPdfA() {
    var oProfile = Preflight.getProfileByName('Convert to PDF/A-1b (sRGB)');
    if (oProfile !== undefined) {
        var myPreflightResult = this.preflight(oProfile, false);
        return this.path;
    }
    console.println("\nPreflight profile not found.");
    return -1;
}
function nUpCircuitCoverOn11by19() {
    // this is based on assumption that original box is letter sized: [0,792,612,0] 
    var orig_box = this.getPageBox('Media');

    // width: need to get to 648 + 720 = 1368 points (19 inches)
    // height: need to get to 792 + 0 = 792 points (11 inches)
    var left_adj = 0;
    var right_adj = 0;
    var curr_width = orig_box[2] - orig_box[0];
    if (curr_width !== 612) {
        var temp_adj = Math.abs((curr_width - 612) / 2);
        if (curr_width < 612) {
            left_adj = -temp_adj;
            right_adj = temp_adj;
        }
        else {
            left_adj = temp_adj;
            right_adj = -temp_adj;
        }
    }

    var top_adj = 0;
    var bottom_adj = 0;
    var curr_height = orig_box[1] - orig_box[3];
    if (curr_height !== 792) {
        temp_adj = Math.abs((curr_height - 792) / 2);
        if (curr_height < 792) {
            top_adj = temp_adj;
            bottom_adj = -temp_adj;
        }
        else {
            top_adj = -temp_adj;
            bottom_adj = temp_adj;
        }
    }

    var p0 = orig_box[0] - 720 + left_adj;
    var p1 = orig_box[1] + top_adj;
    var p2 = orig_box[2] + 36 + right_adj;
    var p3 = orig_box[3] + bottom_adj;
    var m_box = [p0, p1, p2, p3];

    //var m_box = [-720, 792, 648, 0];/*[-612,792,612,0] is tabloid*/
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: m_box
    });
    var copy_to_crop_box = this.getPageBox('Media');
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: copy_to_crop_box
    });
}
function nUpCircuitCoverOn8pt5by23() {
    // this is based on assumption that original box is letter sized: [0,792,612,0] 
    var orig_box = this.getPageBox('Media');

    // width: need to get to 612 + 0 = 612 points (8.5 inches)
    // height: need to get to 792 + 864 = 1656 points (23 inches)
    var left_adj = 0;
    var right_adj = 0;
    var curr_width = orig_box[2] - orig_box[0];
    if (curr_width !== 612) {
        var temp_adj = Math.abs((curr_width - 612) / 2);
        if (curr_width < 612) {
            left_adj = -temp_adj;
            right_adj = temp_adj;
        }
        else {
            left_adj = temp_adj;
            right_adj = -temp_adj;
        }
    }

    var top_adj = 0;
    var bottom_adj = 0;
    var curr_height = orig_box[1] - orig_box[3];
    if (curr_height !== 792) {
        var temp_adj = Math.abs((curr_height - 792) / 2);
        if (curr_height < 792) {
            top_adj = temp_adj;
            bottom_adj = -temp_adj;
        }
        else {
            top_adj = -temp_adj;
            bottom_adj = temp_adj;
        }
    }

    var p0 = orig_box[0] + left_adj;
    var p1 = orig_box[1] + 1656 - 792 + top_adj;
    var p2 = orig_box[2] + right_adj;
    var p3 = orig_box[3] + bottom_adj;
    var m_box = [p0, p1, p2, p3];
    console.println(m_box);

    //var m_box = [0, 0, 612, 1656];
    this.setPageBoxes({
        cBox: 'Media',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: m_box
    });
    var copy_to_crop_box = this.getPageBox('Media');
    this.setPageBoxes({
        cBox: 'Crop',
        nStart: 0,
        nEnd: this.numPages - 1,
        rBox: copy_to_crop_box
    });
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
