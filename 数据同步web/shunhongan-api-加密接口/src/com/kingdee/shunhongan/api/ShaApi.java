package com.kingdee.shunhongan.api;

import java.io.IOException;
import java.io.PrintWriter;
import java.nio.charset.Charset;
import java.security.Key;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Set;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;
import com.kingdee.shunhongan.utils.CryptProperties;
import com.tansun.gateway.utils.cryptsign.CryptSignProperties;
import com.tansun.gateway.utils.cryptsign.KeyUtil;
import com.tansun.gateway.utils.cryptsign.sdk.TansunJsonCryptSignSDKV1;

/**
 * 加密入口类，直接采用了Servlet
 * @author Lam Chan   
 * @date 2018年9月26日 下午2:19:15
 */
public class ShaApi extends HttpServlet {

	/**
	 * 生成公钥和私钥
	 */
	private static final long serialVersionUID = 1L;

	@Override
	protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {

		// 1．调用方生成签名验签用的非对称公私密钥对：
		// 调用提供的jar的方法生成：
		Key[] thisPair = null;
		//2.调用方生成加密用的对称密钥
		Key secretkey = null;
		try {
			thisPair = KeyUtil.createKeyPair();
			secretkey = KeyUtil.createSecretKey("DESede");
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Key publicKey = thisPair[0];
		Key privateKey = thisPair[1];
		// 公钥(BASE64):
		String strPublicKey = KeyUtil.getBase64StringKey(publicKey);
		// 私钥(BASE64):
		String strPrivateKey = KeyUtil.getBase64StringKey(privateKey);
		
		//加密对称密钥(BASE64): 
		String strSecretKey = KeyUtil.getBase64StringKey(secretkey);
		
		// 封装map
		Map<String, String> keyMap = new HashMap<>();
		keyMap.put("publicKey", strPublicKey);
		keyMap.put("privateKey", strPrivateKey);
		keyMap.put("secretKey", strSecretKey);
		
		// 获取公钥和私钥后，写入到配置文件，供签名验签用
		CryptProperties.updatePro("PrivateKey", strPrivateKey);
		CryptProperties.updatePro("PublicKey", strPublicKey);
		//获取秘钥后，写入到配置文件，供加密用
		CryptProperties.updatePro("SecretKey", strSecretKey);
		
		// map转json数据
		String json = JSON.toJSONString(keyMap);

		System.out.println(json);
		PrintWriter out = resp.getWriter();
		out.print(json);
	}

	/**
	 * 用公钥私钥把传过来的数据加密
	 */
	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		// 编码
		req.setCharacterEncoding("utf-8");
		resp.setContentType("text/html;charset=utf-8");
		// 获取参数
		String strHead = req.getParameter("head");
		String strBody = req.getParameter("body");

		// 第一个参数声明需要加密的节点，第二个参数，声明签名存放节点
		TansunJsonCryptSignSDKV1 sdk = new TansunJsonCryptSignSDKV1("body", "head.Sign");
		Map<String, Object> root = new LinkedHashMap<String, Object>();
		// 用fastjson把String转map
	    JSONObject  jsonObjectHead = JSONObject.parseObject(strHead);
	    Map<String,Object> headMaps = (Map<String,Object>)jsonObjectHead;
	    
	    JSONObject  jsonObjectBody = JSONObject.parseObject(strBody);
	    Map<String,Object> bodyMaps = (Map<String,Object>)jsonObjectBody;

		root.put("head", headMaps);
		root.put("body", bodyMaps);

		String cipherStr = "";
		try {
			System.out.println("=================1.初始化加密配件");
			CryptSignProperties encryptproperties = new CryptSignProperties("crypt_this", "crypt_tansun");
			System.out.println("=================2.调用sdk进行加密");
			String clearStr1 = JSON.toJSONString(root);
			System.out.println("=================2.1加密前明文");
			System.out.println(clearStr1);
			byte[] cipher = sdk.encrypt(clearStr1.getBytes(Charset.forName("UTF-8")), encryptproperties);
			cipherStr = new String(cipher, Charset.forName("UTF-8"));
			System.out.println("=================2.2加密后密文");
			System.out.println(cipherStr);
			
			
			
			//本例中为了验签，crypt_this、crypt_tansun中定义的公私钥是同一个，原则crypt_tansun是由它方提供的
			CryptSignProperties decryptproperties = new CryptSignProperties("crypt_this","crypt_tansun");
			System.out.println("=================3.调用sdk进行解密");
			System.out.println("=================3.1解密前密文");
			System.out.println(cipherStr);
			byte[] clear =  sdk.decrypt(cipherStr.getBytes(Charset.forName("UTF-8")), decryptproperties);
			System.out.println("=================3.2解密后明文");
			String clearStr2 = new String(clear,Charset.forName("UTF-8"));
			System.out.println(clearStr2);
			
			
		} catch (Exception e) {
			e.printStackTrace();
		}

		resp.setContentType("text/plain");
		resp.setCharacterEncoding("gb2312");
		PrintWriter out = resp.getWriter();
		out.print(cipherStr);
	}

}
