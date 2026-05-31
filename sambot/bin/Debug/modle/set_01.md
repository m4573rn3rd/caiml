
Downloading the English Wikipedia ranges from 25 GB to 100+ GB, depending on whether you include images and the software used to view the files. [1] [2] [3]

### Text-Only Versions

- **Compressed (Database Dump):** Approximately **25 GB to 35 GB**. These official text-only files require specialized software (like Kiwix or XOWA) to be searched and read properly.
- **Simple Wikipedia:** If storage is tight, you can download a streamlined version of just the basics (around 2 to 4 GB). [4] [5]

### Versions with Images

- **Compressed (with Media):** Roughly **80 GB to 100 GB** for the entire English version including standard article pictures. [1] [6] [7]

### How to Download

The easiest way to get an offline, easily browsable version is by using Kiwix, which allows you to download fully indexed `.zim` files on your phone or computer. Alternatively, if you are tech-savvy and want the raw database, you can pull the official dumps from Wikimedia Downloads. [1] [2]

The easiest way to get an offline, easily browsable version is by using Kiwix, which allows you to download fully indexed `.zim` files on your phone or computer. Alternatively, if you are tech-savvy and want the raw database, you can pull the official dumps from Wikimedia Downloads. [1] [2]



[1] [https://practicalbetterments.com/download-all-of-wikipedia-on-your-phone/](https://practicalbetterments.com/download-all-of-wikipedia-on-your-phone/#:~:text=Wikipedia%20weighs%20in%20at%20around%20102%20gigabytes,or%20only%20ice%20hockey%20articles%20527.6MB%20.)

[2] [https://github.com/gnosygnu/xowa/issues/58](https://github.com/gnosygnu/xowa/issues/58)

[3] [https://word.tips.net/T000196_File_Sizes_in_Word.html](https://word.tips.net/T000196_File_Sizes_in_Word.html#:~:text=The%20size%20of%20files%20created%20by%20Word,files%20created%20by%20those%20different%20versions.%20\(Tips.Net\))

[4] [https://en.wikipedia.org/wiki/Wikipedia:Size_of_Wikipedia](https://en.wikipedia.org/wiki/Wikipedia:Size_of_Wikipedia)

[5] [https://arstechnica.com/information-technology/2013/11/all-of-wikipedia-can-be-installed-to-your-desktop-in-just-30-hours/](https://arstechnica.com/information-technology/2013/11/all-of-wikipedia-can-be-installed-to-your-desktop-in-just-30-hours/)

[6] [https://www.reddit.com/r/YouShouldKnow/comments/15jt8ef/ysk_its_free_to_download_the_entirety_of/](https://www.reddit.com/r/YouShouldKnow/comments/15jt8ef/ysk_its_free_to_download_the_entirety_of/)

[7] [https://www.reddit.com/r/ios/comments/bjl111/all_of_wikipedia_on_your_device_if_you_have_80gb/](https://www.reddit.com/r/ios/comments/bjl111/all_of_wikipedia_on_your_device_if_you_have_80gb/)


---

You can download the text-only version of Wikipedia using two different methods, depending on how you plan to use it. [1]

The Kiwix method is best if you want to immediately read and search Wikipedia offline like a normal website. The Wikimedia Dump method is best if you are a programmer or data scientist who needs raw data for AI, coding, or text analysis. [2, 3, 4, 5, 6]

---

## Method 1: The Kiwix Method (Recommended for Reading)

This gives you a highly compressed, pre-indexed copy of Wikipedia that works inside a dedicated offline browser. It includes text, tables, and math formulas, but absolutely zero images. [2, 3, 7]

- File Size: ~53 GB
- Format: `.zim` file [2, 7, 8]

How to do it:

1. Download the software: Go to the official Kiwix Download Page and install the Kiwix reader application for your device (available for Windows, Mac, Linux, Android, and iOS). [3, 9, 10]
2. Download the text file:
    
    - Open the Kiwix app, navigate to the internal Local Library, and search for "English Wikipedia". Select the version labeled "nopic" (no pictures).
    - Alternatively, you can bypass the app and download the file directly via your browser or a torrent client from the Kiwix ZIM Storage Repository. Look for files named `wikipedia_en_all_nopic...zim`. [2, 3, 7, 8, 11]
    
3. Open and browse: Launch the Kiwix app, click "Open file", select your downloaded `.zim` file, and you can instantly search and read articles offline. [3, 8, 12, 13, 14]

---

## Method 2: The Official Wikimedia Dump (Best for Data & Code)

This gives you the raw, unformatted database backend containing every single article. It contains text mixed with standard Wikipedia markdown layout code (Wikitext). [4, 15, 16]

- File Size: ~25 GB compressed (extracts to over 100 GB of raw XML text)
- Format: `.xml.bz2` [11, 17]

How to do it:

1. Navigate to the repository: Open the [Wikimedia Database Dumps Directory](https://dumps.wikimedia.org/enwiki/). [11]
2. Select the latest date: Click on the top folder folder labeled with the most recent date (e.g., `20260501/`). [17, 18]
3. Locate the text file: Scroll down the massive list until you find the section for articles. You are looking for the exact file name:  
    `enwiki-[DATE]-pages-articles-multistream.xml.bz2` [4, 11, 18, 19, 20]
4. Download the file: Click the link to download via your browser. _Note: Because the file is massive, using a BitTorrent client (if a torrent link is provided on the page) is highly recommended to prevent the download from failing halfway through._ [2, 11, 21]
5. Extract the text: Do not try to open this file in a normal text editor. You will need a python script like [PlainTextWikipedia on GitHub](https://github.com/daveshap/PlainTextWikipedia) or a processing tool like [wiki2text](https://github.com/rspeer/wiki2text) to scrub out the coding markup and extract the clean, raw prose. [20, 22]

What type of device are you downloading this on, and what is your primary goal for the offline text? I can give you instructions on how to set up tools for that specific setup.

  

[1] [https://www.makeuseof.com](https://www.makeuseof.com/tag/guide-downloading-pages-wikipedia/)

[2] [https://www.instructables.com](https://www.instructables.com/Download-Wikipedia-for-Offline-Use/)

[3] [https://diff.wikimedia.org](https://diff.wikimedia.org/2012/04/09/download-the-text-of-the-entire-english-wikipedia/)

[4] [https://medium.com](https://medium.com/neural-engineer/download-the-complete-wikipedia-knowledge-base-for-large-scale-semantic-search-and-ai-applications-69217ae78c09)

[5] [https://a3nm.net](https://a3nm.net/blog/kiwix.html)

[6] [https://github.com](https://github.com/eberlitz/pt-br-corpus)

[7] [https://www.reddit.com](https://www.reddit.com/r/Kiwix/comments/1mpjlww/what_are_all_the_different_versions/)

[8] [https://forums.bunsenlabs.org](https://forums.bunsenlabs.org/viewtopic.php?id=9452)

[9] [https://www.digitec.ch](https://www.digitec.ch/en/page/the-omniscient-usb-stick-or-how-to-carry-wikipedia-in-your-pocket-33660)

[10] [https://edutechwiki.unige.ch](https://edutechwiki.unige.ch/en/EduTechWiki_offline)

[11] [https://en.wikipedia.org](https://en.wikipedia.org/wiki/Wikipedia:Database_download)

[12] [https://www.reddit.com](https://www.reddit.com/r/YouShouldKnow/comments/41vfrh/ysk_wikipedias_book_creator_can_create_a_book/)

[13] [https://itsfoss.com](https://itsfoss.com/self-host-web-archives-kiwix/)

[14] [https://diff.wikimedia.org](https://diff.wikimedia.org/2013/04/17/carry-the-entirety-of-wikipedia-in-your-pocket-with-kiwix-for-android/)

[15] [https://stackoverflow.com](https://stackoverflow.com/questions/4452102/how-to-get-plain-text-out-of-wikipedia)

[16] [https://www.dokuwiki.org](https://www.dokuwiki.org/wikitext)

[17] [https://opendata.stackexchange.com](https://opendata.stackexchange.com/questions/21876/how-do-i-download-a-wikipedia-data-dump)

[18] [https://github.com](https://github.com/daveshap/PlainTextWikipedia)

[19] [https://en.wikipedia.org](https://en.wikipedia.org/wiki/Wikipedia:Database_download)

[20] [https://github.com](https://github.com/rspeer/wiki2text)

[21] [https://lists.wikimedia.org](https://lists.wikimedia.org/hyperkitty/list/offline-l@lists.wikimedia.org/thread/OYXNEWK6RKREDT6U7DC3YQRBYJ3HO4HY/?sort=date)

[22] [https://github.com](https://github.com/daveshap/PlainTextWikipedia)