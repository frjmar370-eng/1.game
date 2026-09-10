# Shotgun 3D — Android

لعبة تصويب 3D موجهة للهواتف بنظام Android.

## التقنية
- Unity 2022.3.62f2
- 3D Mobile / C#
- Android APK

## الموجود في النسخة الحالية
- ساحة 3D مولدة برمجيًا.
- لاعب FPS وشوتجن مع recoil وmuzzle flash.
- أهداف 3D متحركة وتهاجم اللاعب.
- إطلاق منتشر، ذخيرة وإعادة تعبئة.
- نقاط، صحة، جولات Waves ووقت.
- Pause / Game Over / Restart.
- تحكم لمس: حركة، نظر، Fire وReload.
- إعداد تلقائي للمشهد الرئيسي وإعدادات Android.

## إخراج APK — خطوة واحدة
1. افتح المشروع في Unity 2022.3.62f2 مع Android Build Support مثبتًا.
2. انتظر حتى ينتهي Unity من استيراد المشروع.
3. من القائمة اختر: `Shotgun 3D > Build Android APK`.
4. إذا لم يكن `Assets/Scenes/Main.unity` موجودًا، ينشئه النظام تلقائيًا.
5. سيخرج الملف إلى: `Builds/Shotgun3D.apk`.

أداة البناء تضبط `Build App Bundle` على false، لذلك يكون الناتج APK وليس AAB. Unity 2022.3 يدعم إخراج APK مباشرة لنظام Android.

## ملاحظات Android
للبناء الفعلي يجب أن تكون Unity مثبتة معها أدوات Android المطلوبة. وللتجربة على الهاتف يمكن استخدام Debug Signing؛ أما النشر العام فيحتاج توقيعًا مخصصًا.
