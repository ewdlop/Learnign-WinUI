using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace SilkDotNetLibrary.DirectX
{

    public class Direct3D11Context : IDirectXContext, IDisposable
    {
        public readonly System.Single[] BackgroundColour = [0.0f, 0.0f, 0.0f, 1.0f];
        public readonly System.Single[] vertices = new System.Single[] {
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            0.5f, 0.5f, 0.0f,
            -0.5f, 0.5f, 0.0f
        };

        public readonly System.UInt32[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };


        protected bool _disposedValue = false;

        protected readonly IWindow _window;
        protected readonly System.UInt32 vertexStride = 3U * sizeof(float);
        protected readonly System.UInt32 vertexOffset = 0U;
        
        protected DXGI? _dxgi = null;
        protected D3D11? _d3d11 = null;
        protected D3DCompiler? _compiler = null;

        const string shaderSource =
        """
        struct vs_in {
            float3 position_local : POS;
        };

        struct vs_out {
            float4 position_clip : SV_POSITION;
        };

        vs_out vs_main(vs_in input) {
            vs_out output = (vs_out)0;
            output.position_clip = float4(input.position_local, 1.0);
            return output;
        }

        float4 ps_main(vs_out input) : SV_TARGET {
            return float4( 1.0, 0.5, 0.2, 1.0 );
        }
        """;

        // These variables are initialized within the Load event.
        // ComPtr is a smart pointer that manages the lifetime of COM objects.
        protected ComPtr<IDXGIAdapter> _adpater = default;
        protected ComPtr<ID3D11Device> _device = default;
        protected ComPtr<ID3D11DeviceContext> _deviceContext = default;
        protected ComPtr<IDXGIFactory2> _factory = default;
        protected IDXGIOutput _output = default;
        protected ComPtr<IDXGISwapChain1> _swapchain = default;
        protected ID3DInclude _vertextShaderInclude = default;
        protected ID3DInclude _pixelShaerInclude = default;
        protected ID3D11ClassLinkage _vertextShaderClassLinkage = default;
        protected ID3D11ClassLinkage _pixelShaderClassLinkage = default;
        protected ComPtr<ID3D11RenderTargetView> _renderTargetView = default;
        protected ID3D11DepthStencilView _depthStencilView = default;
        protected ComPtr<ID3D11Buffer> _vertexBuffer = default;
        protected ComPtr<ID3D11Buffer> _indexBuffer = default;
        protected ComPtr<ID3D11InputLayout> _inputLayout = default;
        protected ComPtr<ID3D11VertexShader> _vertexShader = default;
        protected ComPtr<ID3D11PixelShader> _pixelShader = default;
        protected ComPtr<ID3D11ClassInstance> _vertexShaderClassInstance = default;
        protected ComPtr<ID3D11ClassInstance> _pixelShaderClassInstance = default;
        protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Direct3D11Context(IWindow window)
        {
            _window = window;
        }

        public unsafe virtual void OnLoad()
        {
            //https://github.com/doitsujin/dxvk
            const bool forceDxvk = false;

            _dxgi = DXGI.GetApi(_window, forceDxvk);
            _d3d11 = D3D11.GetApi(_window, forceDxvk);
            _compiler = D3DCompiler.GetApi();

            // Create our D3D11 logical device.
            SilkMarshal.ThrowHResult
            (
                _d3d11.CreateDevice
                (
                    _adpater,
                    D3DDriverType.Hardware,
                    Software: default,
                    Convert.ToUInt32(CreateDeviceFlag.Debug),
                    null,
                    0,
                    D3D11.SdkVersion,
                    ref _device,
                    null,
                    ref _deviceContext
                )
            );

            if (OperatingSystem.IsWindows())
            {
                // Log debug messages for this device (given that we've enabled the debug flag). Don't do this in release code!
                _device.SetInfoQueueCallback(msg =>
                {
                    byte* pMessage = msg.PDescription;
                    Span<byte> message = new Span<byte>(pMessage, Convert.ToInt32(msg.DescriptionByteLength));
                    Console.WriteLine(SilkMarshal.PtrToString(Convert.ToInt32(message.ToArray())) ?? string.Empty);
                }, _cancellationTokenSource.Token);
            }

            // Create our swapchain.
            SwapChainDesc1 swapChainDesc = new SwapChainDesc1
            {
                BufferCount = 2, // double buffered
                Format = Format.FormatB8G8R8A8Unorm,
                BufferUsage = DXGI.UsageRenderTargetOutput,
                SwapEffect = SwapEffect.FlipDiscard,
                SampleDesc = new SampleDesc(1, 0)
            };

            // Create our DXGI factory to allow us to create a swapchain. 
            _factory = _dxgi.CreateDXGIFactory<IDXGIFactory2>();

            
            if(_window.Native?.DXHandle is null)
            {
                Console.WriteLine("Window Handle is null");
                Dispose();
                return;
            }

            // Create the swapchain.
            SilkMarshal.ThrowHResult
            (
                _factory.CreateSwapChainForHwnd
                (
                    _device,
                    _window.Native.DXHandle.Value,
                    in swapChainDesc,
                    null,
                    ref _output,
                    ref _swapchain
                )
            );

            // Create our vertex buffer.
            BufferDesc bufferDesc = new BufferDesc
            {
                ByteWidth = Convert.ToUInt32(vertices.Length * sizeof(System.Single)),
                Usage = Usage.Default,
                BindFlags = Convert.ToUInt32(BindFlag.VertexBuffer)
            };

            fixed (System.Single* vertexData = vertices)
            {
                SubresourceData subresourceData = new SubresourceData
                {
                    PSysMem = vertexData
                };
                SilkMarshal.ThrowHResult(_device.CreateBuffer(in bufferDesc, in subresourceData, ref _vertexBuffer));
            }

            // Create our index buffer.
            bufferDesc = new BufferDesc
            {
                ByteWidth = Convert.ToUInt32(indices.Length * sizeof(uint)),
                Usage = Usage.Default,
                BindFlags = Convert.ToUInt32(BindFlag.IndexBuffer)
            };

            fixed (System.UInt32* indexData = indices)
            {
                var subresourceData = new SubresourceData
                {
                    PSysMem = indexData
                };

                SilkMarshal.ThrowHResult(_device.CreateBuffer(in bufferDesc, in subresourceData, ref _indexBuffer));
            }

            byte[] shaderBytes = Encoding.ASCII.GetBytes(shaderSource);
            // Compile vertex shader.
            ComPtr<ID3D10Blob> vertexCode = default;
            ComPtr<ID3D10Blob> vertexErrors = default;
            HResult hr = _compiler.Compile
            (
                in shaderBytes[0],
                (nuint)shaderBytes.Length,
                nameof(shaderSource),
                null,
                ref _vertextShaderInclude,
                "vs_main",
                "vs_5_0",
                0,
                0,
                ref vertexCode,
                ref vertexErrors
            );

            // Check for compilation errors.
            if (hr.IsFailure)
            {
                if (vertexErrors.Handle is not null)
                {
                    Console.WriteLine(SilkMarshal.PtrToString((nint)vertexErrors.GetBufferPointer()));
                }

                hr.Throw();
            }

            // Compile pixel shader.
            ComPtr<ID3D10Blob> pixelCode = default;
            ComPtr<ID3D10Blob> pixelErrors = default;
            hr = _compiler.Compile
            (
                in shaderBytes[0],
                (nuint)shaderBytes.Length,
                nameof(shaderSource),
                null,
                ref _pixelShaerInclude,
                "ps_main",
                "ps_5_0",
                0,
                0,
                ref pixelCode,
                ref pixelErrors
            );

            // Check for compilation errors.
            if (hr.IsFailure)
            {
                if (pixelErrors.Handle is not null)
                {
                    Console.WriteLine(SilkMarshal.PtrToString((nint)pixelErrors.GetBufferPointer()));
                }

                hr.Throw();
            }

            // Create vertex shader.
            SilkMarshal.ThrowHResult
            (
                _device.CreateVertexShader
                (
                    vertexCode.GetBufferPointer(),
                    vertexCode.GetBufferSize(),
                    ref _vertextShaderClassLinkage,
                    ref _vertexShader
                )
            );

            // Create pixel shader.
            SilkMarshal.ThrowHResult
            (
                _device.CreatePixelShader
                (
                    pixelCode.GetBufferPointer(),
                    pixelCode.GetBufferSize(),
                    ref _pixelShaderClassLinkage,
                    ref _pixelShader
                )
            );

            //Describe the layout of the input data for the shader.
            //POS means position
            fixed (System.Byte* name = SilkMarshal.StringToMemory("POS"))
            {
                InputElementDesc inputElement = new InputElementDesc
                {
                    SemanticName = name,
                    SemanticIndex = 0,
                    Format = Format.FormatR32G32B32Float,
                    InputSlot = 0,
                    AlignedByteOffset = 0,
                    InputSlotClass = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                };

                SilkMarshal.ThrowHResult
                (
                    _device.CreateInputLayout
                    (
                        in inputElement,
                        1,
                        vertexCode.GetBufferPointer(),
                        vertexCode.GetBufferSize(),
                        ref _inputLayout
                    )
                );
            }

            // Clean up any resources.
            vertexCode.Dispose();
            vertexErrors.Dispose();
            pixelCode.Dispose();
            pixelErrors.Dispose();
        }

        public unsafe virtual void OnRender(double dt)
        {
            // Obtain the framebuffer for the swapchain's backbuffer.
            using ComPtr<ID3D11Texture2D> framebuffer = _swapchain.GetBuffer<ID3D11Texture2D>(0);

            // Create a view over the render target.
            SilkMarshal.ThrowHResult
            (
                _device.CreateRenderTargetView(framebuffer, null, ref _renderTargetView)
            );

            // Clear the render target to be all black ahead of rendeirng.
            _deviceContext.ClearRenderTargetView(_renderTargetView, ref BackgroundColour[0]);

            //Update the rasterizer state with the current viewport.
            Viewport viewport = new Viewport(
                0.0f,
                0.0f,
                _window.FramebufferSize.X,
                _window.FramebufferSize.Y,
                0.0f,
                1.0f
            );


            // Update the rasterizer state with the current viewport.
            _deviceContext.RSSetViewports(1, in viewport);

            // Tell the output merger about our render target view.s
            _deviceContext.OMSetRenderTargets(1, ref _renderTargetView, ref _depthStencilView);

            // Update the input assembler to use our shader input layout, and associated vertex & index buffers.
            _deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3D10PrimitiveTopologyTrianglelist);
            _deviceContext.IASetInputLayout(_inputLayout);
            _deviceContext.IASetVertexBuffers(0, 1, ref _vertexBuffer, in vertexStride, in vertexOffset);
            _deviceContext.IASetIndexBuffer(_indexBuffer, Format.FormatR32Uint, 0);

            // Bind our shaders.
            _deviceContext.VSSetShader(_vertexShader, ref _vertexShaderClassInstance, 0);
            _deviceContext.PSSetShader(_pixelShader,  ref _pixelShaderClassInstance,  0);

            // Draw the quad.
            _deviceContext.DrawIndexed(6, 0, 0);

            // Present the drawn image.
            _swapchain.Present(1, 0);

            _renderTargetView.Dispose();
        }

        public virtual void OnStop()
        {
        }

        public virtual void OnUpdate(double dt)
        {
        }


        public virtual void OnFrameBufferResize(Vector2D<int> newSize)
        {
            // If the window resizes, we need to be sure to update the swapchain's back buffers.
            SilkMarshal.ThrowHResult
            (
                _swapchain.ResizeBuffers(0, Convert.ToUInt32(newSize.X), Convert.ToUInt32(newSize.Y), Format.FormatB8G8R8A8Unorm, 0)
            );
        }

        public virtual void Run()
        {
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.FramebufferResize += OnFrameBufferResize;
            _window.Run();
        }

        protected virtual void OnDispose()
        {
            // Dispose of all the COM objects we've created.
            // order matters?
            _adpater.Dispose();
            _factory.Dispose();
            _swapchain.Dispose();
            _device.Dispose();
            _deviceContext.Dispose();
            _renderTargetView.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _vertexShader.Dispose();
            _pixelShader.Dispose();
            _inputLayout.Dispose();
            _vertexShaderClassInstance.Dispose();
            _pixelShaderClassInstance.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
            //_logger.LogInformation("Direct3D11Context Already Disposed...");
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
