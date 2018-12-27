package com.kingdee.shunhongan.utils;

import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.URL;
import java.util.Properties;

/**
 * 秘钥配置文件crypt_this.properties的读写工具类
 * 
 * @author Lam Chan
 * @date 2018年9月26日 下午2:20:28
 */
public class CryptProperties {
	private static Properties pro = new Properties();
	private static URL url = CryptProperties.class.getClassLoader().getResource("crypt_this.properties");

	private CryptProperties() {
		InputStream in = null;
		try {
			in = this.getClass().getClassLoader().getResourceAsStream("crypt_this.properties");
			pro.load(in);
			in.close();
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	private static final CryptProperties configer = new CryptProperties();

	public static CryptProperties getConfiger() {
		return configer;
	}

	public static String getProperty(String key) {
		return pro.getProperty(key);
	}

	// 参数为要要修改的属性名和属性值
	public static Boolean updatePro(String key, String value) {
		if (pro == null) {
			// load(path);
			System.out.println("修改前重新加载一遍");
		}
		System.out.println("获取添加或修改前的属性值：" + key + "=" + pro.getProperty(key));
		pro.setProperty(key, value);
		// 文件输出流
		try {
			System.out.println("url---------" + java.net.URLDecoder.decode(url.getPath()));
			FileOutputStream fos = new FileOutputStream(java.net.URLDecoder.decode(url.getPath()));
			// 将Properties集合保存到流中
			pro.store(fos, "Copyright (c) Boxcode Studio");
			fos.close();// 关闭流
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return false;
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return false;
		}
		System.out.println("获取添加或修改后的属性值：" + key + "=" + pro.getProperty(key));
		return true;
	}

}
