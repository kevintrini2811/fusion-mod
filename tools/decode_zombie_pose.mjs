// Offline asset conversion only; reads Spine bytes, never loads game code.
import fs from 'node:fs';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
const runtime = path.resolve(import.meta.dirname, '../../.tools/spine-offline/node_modules/@esotericsoftware/spine-core/dist/index.js');
const spine = await import(pathToFileURL(runtime));
const [folder, name, output, skin, animation, atlasName] = process.argv.slice(2);
const atlas = new spine.TextureAtlas(fs.readFileSync(path.join(folder, (atlasName || name) + '.atlas'), 'utf8'));
for (const page of atlas.pages) page.setTexture({setFilters() {}, setWraps() {}, getImage() {return {width: page.width, height: page.height};}});
const reader = new spine.SkeletonBinary(new spine.AtlasAttachmentLoader(atlas));
const data = reader.readSkeletonData(new Uint8Array(fs.readFileSync(path.join(folder, name + '.skel'))));
const skeleton = new spine.Skeleton(data);
if (skin && skin !== '-') {
 const combined = new spine.Skin('portrait');
 for (const name of skin.split(',')) {
  const item = data.findSkin(name);
  if (!item) throw new Error('Missing skin: '+name);
  combined.addSkin(item);
 }
 skeleton.setSkin(combined);
}
skeleton.setToSetupPose();
if (animation && animation !== '-') {
 const anim = data.findAnimation(animation);
 if (!anim) throw new Error('Missing animation: ' + animation);
 anim.apply(skeleton, 0, 0, false, [], 1, spine.MixBlend.replace, spine.MixDirection.mixIn);
}
skeleton.updateWorldTransform(spine.Physics.pose);
const attachments = [];
for (const slot of skeleton.drawOrder) {
 const a = slot.getAttachment();
 if (!a || !slot.bone.active) continue;
 let vertices, triangles;
 if (a instanceof spine.RegionAttachment) {
   vertices = new Array(8); a.computeWorldVertices(slot, vertices, 0, 2); triangles = [0,1,2,2,3,0];
 } else if (a instanceof spine.MeshAttachment) {
   vertices = new Array(a.worldVerticesLength); a.computeWorldVertices(slot, 0, vertices.length, vertices, 0, 2); triangles = Array.from(a.triangles);
 } else { if (a instanceof spine.ClippingAttachment) throw new Error('Clipping needs explicit handling: '+slot.data.name); continue; }
 attachments.push({slot:slot.data.name, bone:slot.bone.data.name, name:a.name, page:a.region.page.name,
   vertices, triangles, uvs:Array.from(a.uvs), color:[slot.color.r*a.color.r,slot.color.g*a.color.g,slot.color.b*a.color.b,slot.color.a*a.color.a]});
}
fs.writeFileSync(output, JSON.stringify({name,skins:data.skins.map(s=>s.name),animations:data.animations.map(a=>a.name),
 skinAttachments:data.skins.map(s=>({name:s.name,bones:s.bones.map(b=>b.name),entries:s.getAttachments().map(e=>({slot:e.slotIndex,name:e.name}))})),
 debugSlots:skeleton.slots.map(s=>({name:s.data.name,active:s.bone.active,attachment:s.getAttachment()?.name,type:s.getAttachment()?.constructor.name})),
 slots:data.slots.map(s=>({name:s.name,attachment:s.attachmentName})),
 bones:skeleton.bones.map(b=>({name:b.data.name,parent:b.parent?.data.name,x:b.worldX,y:b.worldY,a:b.a,b:b.b,c:b.c,d:b.d})),attachments},null,2));
console.log(name, attachments.length, 'attachments;', data.skins.map(s=>s.name).join(','), '; animations:', data.animations.map(a=>a.name).join(','));
