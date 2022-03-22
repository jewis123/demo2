using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

namespace FrameGraph.Example
{
    public class GraphTest : MonoBehaviour
    {
        void Start()
        {
            TestSequence();
            TestDisorder();
            TestBranch();
            TestTwoReaders();
            TestErrorMulitPassWriteResource();
            TestErrorInvalidInputResource();
            TestErrorResourceLifeTime();
            TestErrorRenderGraphRing();
        }

        private void TestSequence()
        {
            Debug.Log("================== [Test Sequence] ==================");
            FrameGraph fg = new FrameGraph();

            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var depthTexture = fg.RegisterResourceNode("DepthTexture");
            var gbuffer1 = fg.RegisterResourceNode("GBuffer1");
            var gbuffer2 = fg.RegisterResourceNode("GBuffer2");
            var gbuffer3 = fg.RegisterResourceNode("GBuffer3");
            var finaltarget = fg.RegisterResourceNode("Final Target");

            var gBufferPass = fg.RegisterPassNode("GBuffer pass", new int[] { }, new[] {depthAttachment, gbuffer1, gbuffer2, gbuffer3});
            var copyDepthPass = fg.RegisterPassNode("Copy depth", new[] {depthAttachment}, new[] {depthTexture});
            var lightingPass = fg.RegisterPassNode("Lighting", new int[] {depthTexture, gbuffer1, gbuffer2, gbuffer3}, new int[] {finaltarget});
            var skyBoxPass = fg.RegisterPassNode("SkyBox", new int[] { }, new int[] {finaltarget});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] { }, new int[] {finaltarget});

            fg.RegisterResRefPassNode(copyDepthPass, new[] {gBufferPass}, new[] {PassNode.NO_PASS});
            fg.RegisterResRefPassNode(lightingPass, new[] {copyDepthPass, gBufferPass, gBufferPass, gBufferPass}, new[] {PassNode.NO_PASS});
            fg.RegisterResRefPassNode(skyBoxPass, new int[] { }, new[] {lightingPass});
            fg.RegisterResRefPassNode(transparentPass, new int[] { }, new[] {skyBoxPass});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == 0);

            List<int> expectedList = new List<int>();
            expectedList.Add(gBufferPass);
            expectedList.Add(copyDepthPass);
            expectedList.Add(lightingPass);
            expectedList.Add(skyBoxPass);
            expectedList.Add(transparentPass);

            TestDebug.AssertList(expectedList, crst.sortedPasses);
            //TestDebug.PrintSortedPasses(crst, fg);
        }

        private void TestDisorder()
        {
            Debug.Log("================== [Test Disorder] ==================");
            FrameGraph fg = new FrameGraph();

            var finaltarget = fg.RegisterResourceNode("Final Target");
            var gbuffer1 = fg.RegisterResourceNode("GBuffer1");
            var gbuffer2 = fg.RegisterResourceNode("GBuffer2");
            var gbuffer3 = fg.RegisterResourceNode("GBuffer3");
            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var depthTexture = fg.RegisterResourceNode("DepthTexture");

            var copyDepthPass = fg.RegisterPassNode("Copy depth", new[] {depthAttachment}, new[] {depthTexture});
            var skyBoxPass = fg.RegisterPassNode("SkyBox", new int[] { }, new int[] {finaltarget});
            var gBufferPass = fg.RegisterPassNode("GBuffer pass", new int[] { }, new[] {depthAttachment, gbuffer1, gbuffer2, gbuffer3});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] { }, new int[] {finaltarget});
            var lightingPass = fg.RegisterPassNode("Lighting", new int[] {depthTexture, gbuffer1, gbuffer2, gbuffer3}, new int[] {finaltarget});

            fg.RegisterResRefPassNode(lightingPass, new[] {copyDepthPass, gBufferPass, gBufferPass, gBufferPass}, new[] {PassNode.NO_PASS});
            fg.RegisterResRefPassNode(transparentPass, new int[] { }, new[] {skyBoxPass});
            fg.RegisterResRefPassNode(copyDepthPass, new[] {gBufferPass}, new[] {PassNode.NO_PASS});
            fg.RegisterResRefPassNode(skyBoxPass, new int[] { }, new[] {lightingPass});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == 0);

            List<int> expectedList = new List<int>();
            expectedList.Add(gBufferPass);
            expectedList.Add(copyDepthPass);
            expectedList.Add(lightingPass);
            expectedList.Add(skyBoxPass);
            expectedList.Add(transparentPass);

            TestDebug.AssertList(expectedList, crst.sortedPasses);
            // TestDebug.PrintSortedPasses(crst, fg);
        }

        private void TestBranch()
        {
            Debug.Log("================== [Test Branch] ==================");
            FrameGraph fg = new FrameGraph();

            var colorAttachment = fg.RegisterResourceNode("ColorAttachment");
            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var finaltarget = fg.RegisterResourceNode("Final Target");

            var opaquePass = fg.RegisterPassNode("Opaque", new int[] { }, new[] {colorAttachment, depthAttachment});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] { }, new int[] {colorAttachment, depthAttachment});
            var waterOnlyPass = fg.RegisterPassNode("WaterOnly", new int[] {depthAttachment}, new int[] {colorAttachment, finaltarget});

            fg.RegisterResRefPassNode(transparentPass, new int[] { }, new[] {waterOnlyPass, opaquePass});
            fg.RegisterResRefPassNode(waterOnlyPass, new[] {opaquePass}, new[] {opaquePass, PassNode.NO_PASS});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == 0);

            List<int> expectedList = new List<int>();
            expectedList.Add(opaquePass);
            expectedList.Add(waterOnlyPass);
            expectedList.Add(transparentPass);

            TestDebug.AssertList(expectedList, crst.sortedPasses);
            //TestDebug.PrintSortedPasses(crst, fg);
        }

        private void TestTwoReaders()
        {
            Debug.Log("================== [Test TwoReaders] ==================");
            FrameGraph fg = new FrameGraph();

            var colorAttachment = fg.RegisterResourceNode("ColorAttachment");
            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var shadowMapTex = fg.RegisterResourceNode("ShadowMapTex");

            var shadowPass = fg.RegisterPassNode("ShadowCaster", new int[] { }, new[] {shadowMapTex});
            var opaquePass = fg.RegisterPassNode("Opaque", new int[] {shadowMapTex}, new[] {colorAttachment, depthAttachment});
            var skyboxPass = fg.RegisterPassNode("SkyBox", new int[] { }, new int[] {colorAttachment, depthAttachment});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] {shadowMapTex}, new int[] {colorAttachment, depthAttachment});

            fg.RegisterResRefPassNode(opaquePass, new int[] {shadowPass}, new[] {PassNode.NO_PASS, PassNode.NO_PASS});
            fg.RegisterResRefPassNode(skyboxPass, new int[] { }, new[] {opaquePass, opaquePass});
            fg.RegisterResRefPassNode(transparentPass, new int[] {shadowPass}, new[] {skyboxPass, skyboxPass});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == 0);

            List<int> expectedList = new List<int>();
            expectedList.Add(shadowPass);
            expectedList.Add(opaquePass);
            expectedList.Add(transparentPass);

            TestDebug.AssertList(expectedList, crst.res2Info[shadowMapTex].trimmedSortedList);
            //TestDebug.PrintSortedPasses(crst, fg);
        }

        private void TestErrorMulitPassWriteResource()
        {
            Debug.Log("================== [Test Error MulitPass Write Resource] ==================");
            FrameGraph fg = new FrameGraph();

            var colorAttachment = fg.RegisterResourceNode("ColorAttachment");
            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var finaltarget = fg.RegisterResourceNode("Final Target");

            var opaquePass = fg.RegisterPassNode("Opaque", new int[] { }, new[] {colorAttachment, depthAttachment});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] { }, new int[] {colorAttachment, depthAttachment});
            var waterOnlyPass = fg.RegisterPassNode("WaterOnly", new int[] { }, new int[] {colorAttachment, depthAttachment});

            fg.RegisterResRefPassNode(waterOnlyPass, new int[] { }, new[] {opaquePass, opaquePass});
            fg.RegisterResRefPassNode(transparentPass, new int[] { }, new[] {waterOnlyPass, opaquePass});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == FrameGraphCompiler.Result.ERROR_MULIT_PASS_WRITE_RESOURCE);
        }

        private void TestErrorInvalidInputResource()
        {
            Debug.Log("================== [Test Error Invalid Input Resource] ==================");
            FrameGraph fg = new FrameGraph();

            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var finaltarget = fg.RegisterResourceNode("Final Target");

            var opaquePass = fg.RegisterPassNode("Opaque", new int[] {depthAttachment}, new[] {finaltarget});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == FrameGraphCompiler.Result.ERROR_INVALID_INPUT_RESOURCE);
        }

        private void TestErrorResourceLifeTime()
        {
            Debug.Log("================== [Test Error Resource Life Time] ==================");
            FrameGraph fg = new FrameGraph();

            var colorAttachment = fg.RegisterResourceNode("ColorAttachment");
            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");
            var finaltarget = fg.RegisterResourceNode("Final Target");

            var opaquePass = fg.RegisterPassNode("Opaque", new int[] { }, new[] {colorAttachment, depthAttachment});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] { }, new int[] {colorAttachment, depthAttachment});
            var waterOnlyPass = fg.RegisterPassNode("WaterOnly", new int[] {depthAttachment}, new int[] {colorAttachment});

            fg.RegisterResRefPassNode(waterOnlyPass, new int[] {opaquePass}, new[] {transparentPass});
            fg.RegisterResRefPassNode(transparentPass, new int[] { }, new[] {opaquePass, opaquePass});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == FrameGraphCompiler.Result.ERROR_RESOURCE_LIFE_TIME);
        }

        private void TestErrorRenderGraphRing()
        {
            Debug.Log("================== [Test Error Render Graph Ring] ==================");
            FrameGraph fg = new FrameGraph();

            var colorAttachment = fg.RegisterResourceNode("ColorAttachment");
            var depthAttachment = fg.RegisterResourceNode("DepthAttachment");

            var opaquePass = fg.RegisterPassNode("Opaque", new int[] { }, new[] {colorAttachment, depthAttachment});
            var transparentPass = fg.RegisterPassNode("Transparent", new int[] { }, new int[] {colorAttachment, depthAttachment});
            var waterOnlyPass = fg.RegisterPassNode("WaterOnly", new int[] { }, new int[] {colorAttachment, depthAttachment});

            fg.RegisterResRefPassNode(opaquePass, new int[] { }, new[] {transparentPass, transparentPass});
            fg.RegisterResRefPassNode(waterOnlyPass, new int[] { }, new[] {opaquePass, opaquePass});
            fg.RegisterResRefPassNode(transparentPass, new int[] { }, new[] {waterOnlyPass, waterOnlyPass});

            FrameGraphCompiler compiler = new FrameGraphCompiler();
            var crst = compiler.Compile(fg);

            Assert.IsTrue(crst.error == FrameGraphCompiler.Result.ERROR_RENDERGRAPH_RING);
        }
    }
}